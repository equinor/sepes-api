﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetCloudResourceService : IDatasetCloudResourceService
    {
        readonly IConfiguration _config;
        readonly SepesDbContext _db;
        readonly ILogger<DatasetCloudResourceService> _logger;

        readonly IUserService _userService;

        readonly IStudyEfModelService _studyModelService;

        readonly ICloudResourceCreateService _cloudResourceCreateService;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceOperationReadService _cloudResourceOperationReadService;
        readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;
        readonly IProvisioningQueueService _provisioningQueueService;

        readonly IDatasetFirewallService _datasetFirewallService;
        readonly IDatasetWaitForFirewallOperationService _datasetWaitForFirewallOperationService;

        public DatasetCloudResourceService(
            IConfiguration config,
            SepesDbContext db,
            ILogger<DatasetCloudResourceService> logger,
           IUserService userService,
           IStudyEfModelService studyModelService,
           ICloudResourceCreateService cloudResourceCreateService,
           ICloudResourceReadService cloudResourceReadService,
           ICloudResourceOperationReadService cloudResourceOperationReadService,
           ICloudResourceOperationCreateService cloudResourceOperationCreateService,
           IProvisioningQueueService provisioningQueueService,
            IDatasetFirewallService datasetFirewallService,
            IDatasetWaitForFirewallOperationService datasetWaitForFirewallOperationService)
        {
            _config = config;
            _db = db;
            _logger = logger;
            _userService = userService;
            _studyModelService = studyModelService;
            _cloudResourceCreateService = cloudResourceCreateService;
            _cloudResourceReadService = cloudResourceReadService;
            _cloudResourceOperationReadService = cloudResourceOperationReadService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _provisioningQueueService = provisioningQueueService;
            _datasetFirewallService = datasetFirewallService;
            _datasetWaitForFirewallOperationService = datasetWaitForFirewallOperationService;
        }

        public async Task CreateResourceGroupForStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default)
        {
            var studyForCreation = await _studyModelService.GetForDatasetCreationNoAccessCheckAsync(study.Id);
            var resourceGroupForDatasets = GetResourceGroupForStudySpecificDataset(studyForCreation);

            var parentQueueItem = QueueItemFactory.CreateParent("Create resource group for Study specific datasets");

            if (resourceGroupForDatasets == null)
            {
                var resourceGroupName = AzureResourceNameUtil.StudySpecificDatasetResourceGroup(studyForCreation.Name);
                var tags = ResourceTagFactory.StudySpecificDatasourceResourceGroupTags(_config, studyForCreation);
                resourceGroupForDatasets = await _cloudResourceCreateService.CreateStudySpecificResourceGroupEntryAsync(studyForCreation.Id, resourceGroupName, "norwayeast", tags);
                ProvisioningQueueUtil.CreateChildAndAdd(parentQueueItem, resourceGroupForDatasets);
            }
            else
            {
                throw new Exception("Resource group allready exists");
            }

            await ScheduleResourceGroupRoleAssignments(studyForCreation, resourceGroupForDatasets, parentQueueItem);

            await _provisioningQueueService.SendMessageAsync(parentQueueItem, cancellationToken: cancellationToken);
        }


        public async Task CreateResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, string clientIp, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

            var parentQueueItem = QueueItemFactory.CreateParent("Create resources for Study Specific Dataset");
            var resourceGroupDb = await EnsureResourceGroupExists(study, dataset, parentQueueItem);

            await OrderCreationOfStudySpecificDatasetStorageAccount(study, dataset, resourceGroupDb, clientIp, parentQueueItem, cancellationToken);

            await _provisioningQueueService.SendMessageAsync(parentQueueItem, cancellationToken: cancellationToken);
        }

        async Task<CloudResource> EnsureResourceGroupExists(Study study, Dataset dataset, ProvisioningQueueParentDto parentQueueItem)
        {
            try
            {
                var resourceGroupDb = GetResourceGroupForStudySpecificDataset(study);

                if (resourceGroupDb == null)
                {
                    resourceGroupDb = await CreateResourceGroupForStudySpecificDatasetsInternalAsync(study, parentQueueItem);
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(resourceGroupDb.LastKnownProvisioningState))
                    {
                        var resourceGroupCreateOperation = CloudResourceOperationUtil.GetCreateOperation(resourceGroupDb);

                        //Old and failed, give it another try
                        if (resourceGroupCreateOperation != null
                            && resourceGroupCreateOperation.Created.AddMinutes(10) <= DateTime.UtcNow
                            && (resourceGroupCreateOperation.Status == CloudResourceOperationState.FAILED || resourceGroupCreateOperation.Status == CloudResourceOperationState.ABORTED))
                        {
                            ProvisioningQueueUtil.CreateChildAndAdd(parentQueueItem, resourceGroupDb);
                        }
                    }
                    else
                    {
                        if (resourceGroupDb.LastKnownProvisioningState == CloudResourceProvisioningStates.SUCCEEDED || resourceGroupDb.LastKnownProvisioningState == CloudResourceProvisioningStates.CREATING)
                        {
                            return resourceGroupDb;
                        }
                    }
                }

                return resourceGroupDb;

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to locate or ensure that resource group exist for storage account", ex);
            }
        }

        async Task<CloudResource> CreateResourceGroupForStudySpecificDatasetsInternalAsync(Study study, ProvisioningQueueParentDto parentQueueItem)
        {
            var resourceGroupName = AzureResourceNameUtil.StudySpecificDatasetResourceGroup(study.Name);
            var tags = ResourceTagFactory.StudySpecificDatasourceResourceGroupTags(_config, study);
            var resourceGroupForDatasets = await _cloudResourceCreateService.CreateStudySpecificResourceGroupEntryAsync(study.Id, resourceGroupName, "norwayeast", tags);
            ProvisioningQueueUtil.CreateChildAndAdd(parentQueueItem, resourceGroupForDatasets);

            await ScheduleResourceGroupRoleAssignments(study, resourceGroupForDatasets, parentQueueItem);

            return resourceGroupForDatasets;
        }

        async Task<CloudResource> GetResourceGroupEntryForStudySpecificDatasetDeletionAsync(Study study)
        {
            return await _cloudResourceReadService.GetByStudyIdForDeletionNoAccessCheckAsync(study.Id);
        }

        CloudResource GetResourceGroupForStudySpecificDataset(Study study, bool includeDeleted = false)
        {
            if (study.Resources == null)
            {
                throw new Exception("Missing Include for CloudResource on Study");
            }

            foreach (var curResource in study.Resources.Where(r=> !SoftDeleteUtil.IsMarkedAsDeleted(r) || includeDeleted))
            {
                if (curResource.ResourceType == AzureResourceType.ResourceGroup)
                {
                    if (!String.IsNullOrWhiteSpace(curResource.Purpose) && curResource.Purpose == CloudResourcePurpose.StudySpecificDatasetContainer)
                    {
                        return curResource;
                    }
                }
            }

            return null;
        }

        async Task ScheduleResourceGroupRoleAssignments(Study study, CloudResource resourceGroup, ProvisioningQueueParentDto queueParentItem)
        {
            var desiredState = CloudResourceConfigStringSerializer.Serialize(new CloudResourceOperationStateForRoleUpdate(study.Id));

            var resourceGroupCreateOperation = CloudResourceOperationUtil.GetCreateOperation(resourceGroup);

            var roleAssignmentUpdateOperation = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES, dependsOn: resourceGroupCreateOperation.Id, desiredState: desiredState);

            ProvisioningQueueUtil.CreateChildAndAdd(queueParentItem, roleAssignmentUpdateOperation);
        }

        async Task OrderCreationOfStudySpecificDatasetStorageAccount(Study study, Dataset dataset, CloudResource resourceGroup, string clientIp, ProvisioningQueueParentDto queueParent, CancellationToken cancellationToken)
        {
            try
            {
                if (resourceGroup == null)
                {
                    throw new ArgumentNullException("resourceGroup", "Resource group entry is null");
                }

                _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

                var tagsForStorageAccount = ResourceTagFactory.StudySpecificDatasourceStorageAccountTags(_config, study, dataset.Name);
                var storageAccountName = AzureResourceNameUtil.StudySpecificDataSetStorageAccount(dataset.Name);

                var resourceEntry = await _cloudResourceCreateService.CreateStudySpecificDatasetEntryAsync(dataset.Id, resourceGroup.Id, resourceGroup.Region, resourceGroup.ResourceGroupName, storageAccountName, tagsForStorageAccount);

                ProvisioningQueueUtil.CreateChildAndAdd(queueParent, resourceEntry);

                await _datasetFirewallService.EnsureDatasetHasFirewallRules(dataset, clientIp);

                await _db.SaveChangesAsync();

                var stateForFirewallOperation = TranslateAllowedIpsToOperationDesiredState(dataset.FirewallRules.ToList());

                var createStorageAccountOperation = CloudResourceOperationUtil.GetCreateOperation(resourceEntry);
                var firewallUpdateOperation = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceEntry.Id, CloudResourceOperationType.ENSURE_FIREWALL_RULES, dependsOn: createStorageAccountOperation.Id, desiredState: stateForFirewallOperation);

                ProvisioningQueueUtil.CreateChildAndAdd(queueParent, firewallUpdateOperation);

                var stateForCorsRules = DatasetCorsUtils.CreateDatasetCorsRules(_config);
                var corsUpdateOperation = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceEntry.Id, CloudResourceOperationType.ENSURE_CORS_RULES, dependsOn: firewallUpdateOperation.Id, desiredState: stateForCorsRules);

                ProvisioningQueueUtil.CreateChildAndAdd(queueParent, corsUpdateOperation);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to schedule creation of Azure Storage Account", ex);
            }
        }

        public async Task EnsureFirewallExistsAsync(Study study, Dataset dataset, string clientIp, CancellationToken cancellationToken = default)
        {
            if (await _datasetFirewallService.SetDatasetFirewallRules(dataset, clientIp))
            {
                await _db.SaveChangesAsync(cancellationToken);

                var stateForFirewallOperation = TranslateAllowedIpsToOperationDesiredState(dataset.FirewallRules.ToList());
                var datasetStorageAccountResource = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);
                var firewallUpdateOperation = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(datasetStorageAccountResource.Id,
                    CloudResourceOperationType.ENSURE_FIREWALL_RULES, desiredState: stateForFirewallOperation);

                await _provisioningQueueService.CreateItemAndEnqueue(firewallUpdateOperation);

                await _datasetWaitForFirewallOperationService.WaitForOperationToCompleteAsync(firewallUpdateOperation.Id);
            }
        }

        public async Task DeleteAllStudyRelatedResourcesAsync(Study study, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"DeleteAllStudyRelatedResourcesAsync - Study Id: {study.Id}");

            try
            {
                var resourceGroupEntry = await GetResourceGroupEntryForStudySpecificDatasetDeletionAsync(study);

                if (resourceGroupEntry != null)
                {
                    var currentUser = await _userService.GetCurrentUserAsync();

                    SoftDeleteUtil.MarkAsDeleted(resourceGroupEntry, currentUser);

                    foreach (var curResource in resourceGroupEntry.ChildResources)
                    {
                        SoftDeleteUtil.MarkAsDeleted(curResource, currentUser);
                    }

                    var deleteOperation = await _cloudResourceOperationCreateService.CreateDeleteOperationAsync(resourceGroupEntry.Id, $"Delete study related resurces for Study {study.Id}");
                    await _provisioningQueueService.CreateItemAndEnqueue(deleteOperation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete resources for study specific dataset", ex);
            }
        }

        public async Task DeleteResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"DeleteResourcesForStudySpecificDatasetAsync - Dataset Id: {dataset.Id}. Resources will be marked ");

            try
            {
                var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);

                if (datasetResourceEntry != null)
                {
                    await SoftDeleteUtil.MarkAsDeleted(datasetResourceEntry, _userService);

                    var deleteOperation = await _cloudResourceOperationCreateService.CreateDeleteOperationAsync(datasetResourceEntry.Id, $"Delete dataset related resurces for dataset {dataset.Id}");
                    await _provisioningQueueService.CreateItemAndEnqueue(deleteOperation);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete resources for study specific dataset", ex);
            }
        }

        string TranslateAllowedIpsToOperationDesiredState(List<DatasetFirewallRule> datasetFirewallRules)
        {
            if (datasetFirewallRules.Count == 0)
            {
                return null;
            }

            var translated = TranslateAllowedIpsToGenericFirewallRule(datasetFirewallRules);

            return CloudResourceConfigStringSerializer.Serialize(translated);
        }

        List<FirewallRule> TranslateAllowedIpsToGenericFirewallRule(List<DatasetFirewallRule> datasetFirewallRules)
        {
            return datasetFirewallRules.Select(r => new FirewallRule() { Action = FirewallRuleAction.Allow, Address = r.Address }).ToList();
        }
    }
}
