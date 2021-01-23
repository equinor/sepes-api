using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
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

        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceCreateService _cloudResourceCreateService;
        readonly ICloudResourceDeleteService _cloudResourceDeleteService;   

        public DatasetCloudResourceService(IConfiguration config, SepesDbContext db, ILogger<DatasetCloudResourceService> logger,
           IUserService userService,
           ICloudResourceReadService cloudResourceReadService, ICloudResourceCreateService cloudResourceCreateService, ICloudResourceDeleteService cloudResourceDeleteService)
        {
            _config = config;
            _db = db;
            _logger = logger;
            _userService = userService;

            _cloudResourceReadService = cloudResourceReadService;
            _cloudResourceCreateService = cloudResourceCreateService;
            _cloudResourceDeleteService = cloudResourceDeleteService;          
        }

        //public async Task CreateResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, string clientIp, CancellationToken cancellationToken = default)
        //{
        //    _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

        //    await CreateStorageAccountForStudySpecificDatasets(dataset, clientIp, cancellationToken);
        //    await AddRoleAssignmentForCurrentUser(dataset, cancellationToken);
        //}

        public async Task CreateResourcesForStudySpecificDatasetAsync(Dataset dataset, string clientIp, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

            var resourceGroupDbId = await EnsureResourceGroupForStudySpecificDatasetExistsAsync(dataset.Study);
            await CreateStorageAccountForStudySpecificDatasets(dataset, resourceGroupDbId, clientIp, cancellationToken);
            await AddRoleAssignmentForCurrentUser(dataset, cancellationToken);
        }

        public async Task EnsureExistFirewallExceptionForApplication(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var serverRule = await CreateServerRule(currentUser);

            bool serverRuleAllreadyExist = false;

            var rulesToRemove = new List<DatasetFirewallRule>();
            //Get firewall rules from  database, determine of some have to be deleted
            foreach (var curDbRule in dataset.FirewallRules)
            {
                if (curDbRule.RuleType == DatasetFirewallRuleType.Api)
                {
                    if (curDbRule.Address == serverRule.Address)
                    {
                        serverRuleAllreadyExist = true;
                    }
                    else
                    {
                        if (curDbRule.Created.AddMonths(1) < DateTime.UtcNow)
                        {
                            rulesToRemove.Add(curDbRule);
                        }
                    }
                }
            }

            foreach (var curRuleToRemove in rulesToRemove)
            {
                dataset.FirewallRules.Remove(curRuleToRemove);
            }

            if (!serverRuleAllreadyExist)
            {
                dataset.FirewallRules.Add(serverRule);
            }

            await _db.SaveChangesAsync(cancellationToken);

            await _azureStorageAccountService.SetStorageAccountAllowedIPs(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, dataset.FirewallRules.Select(fw => fw.Address).ToList(), cancellationToken);
        }

        public async Task DeleteResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"DeleteResourcesForStudySpecificDatasetAsync - Dataset Id: {dataset.Id}");

            try
            {
                if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup) == false && String.IsNullOrWhiteSpace(dataset.StorageAccountName) == false)
                {
                    await _azureStorageAccountService.DeleteStorageAccount(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, cancellationToken);
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete resources for study specific dataset", ex);
            }
        }

        public async Task DeleteAllStudyRelatedResourcesAsync(Study study, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"DeleteAllStudyRelatedResourcesAsync - Study Id: {study.Id}");

            try
            {
                if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup) == false)
                {
                    await _azureResourceGroupService.Delete(study.StudySpecificDatasetsResourceGroup, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete resources for study specific dataset", ex);
            }
        }

        CloudResource GetResourceGroupForStudySpecificDataset(Study study)
        {
            if (study.Resources == null)
            {
                throw new Exception("Missing Include for CloudResource on Study");
            }

            foreach (var curResource in study.Resources)
            {
                if (SoftDeleteUtil.IsMarkedAsDeleted(curResource) == false)
                {
                    if (curResource.ResourceType == AzureResourceType.ResourceGroup)
                    {
                        if (String.IsNullOrWhiteSpace(curResource.Purpose) == false && curResource.Purpose == CloudResourcePurpose.StudySpecificDataset)
                        {
                            return curResource;
                        }
                    }
                }
            }

            return null;
        }

        async Task<int> EnsureResourceGroupForStudySpecificDatasetExistsAsync(Study study)
        {
            var datasetResourceGroupEntry = GetResourceGroupForStudySpecificDataset(study);

            if (datasetResourceGroupEntry == null)
            {
                //Create resource group entry
                //Set a new name                
            }

        

            if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup))
            {
                study.StudySpecificDatasetsResourceGroup = AzureResourceNameUtil.StudySpecificDatasetResourceGroup(study.Name);
            }

            var tagsForResourceGroup = AzureResourceTagsFactory.StudySpecificDatasourceResourceGroupTags(_config, study);
            return datasetResourceGroupEntry.Id;

            //await _azureResourceGroupService.EnsureCreated(study.StudySpecificDatasetsResourceGroup, RegionStringConverter.Convert(dataset.Location), tagsForResourceGroup, cancellationToken);
        }

        async Task CreateStorageAccountForStudySpecificDatasets(Dataset dataset, int storageAccountId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

             

            

                //TODO: Create resource entry

               
                var currentUser = await _userService.GetCurrentUserAsync();

                dataset.FirewallRules = new List<DatasetFirewallRule>();

                //Add user's client IP

                if (clientIp != "::1")
                {
                    dataset.FirewallRules.Add(CreateRule(currentUser, DatasetFirewallRuleType.Client, clientIp));
                }               

                //Add Sepes IP, so that it can uload/download files 
                var serverPublicIp = await IpAddressUtil.GetServerPublicIp();
                dataset.FirewallRules.Add(await CreateServerRule(currentUser));

                var tagsForStorageAccount = AzureResourceTagsFactory.StudySpecificDatasourceStorageAccountTags(_config, study, dataset.Name);
                var newStorageAccount = await _azureStorageAccountService.CreateStorageAccount(RegionStringConverter.Convert(dataset.Location), study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, tagsForStorageAccount, onlyAllowAccessFrom: dataset.FirewallRules.Select(fw => fw.Address).ToList(), cancellationToken);

                dataset.StorageAccountId = newStorageAccount.Id;
                dataset.StorageAccountName = newStorageAccount.Name;
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Azure Storage Account", ex);
            }
        }

        async Task<DatasetFirewallRule> CreateServerRule(UserDto user)
        {
            var serverPublicIp = await IpAddressUtil.GetServerPublicIp();
            return CreateRule(user, DatasetFirewallRuleType.Api, serverPublicIp);
        }

        DatasetFirewallRule CreateRule(UserDto user, DatasetFirewallRuleType ruleType, string ipAddress)
        {
            return new DatasetFirewallRule() { CreatedBy = user.UserName, RuleType = ruleType, Address = ipAddress, Created = DateTime.UtcNow };
        }

        async Task AddRoleAssignmentForCurrentUser(Dataset dataset, CancellationToken cancellationToken = default)
        {
            try
            {

                var currentUser = await _userService.GetCurrentUserAsync();

                var roleDefinitionId = AzureRoleIds.CreateRoleDefinitionUrl(dataset.StorageAccountId, AzureRoleIds.READ);
                await _azureRoleAssignmentService.AddRoleAssignment(dataset.StorageAccountId, roleDefinitionId, currentUser.ObjectId, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create Role Assignment for Storage Account", ex);
            }
        }
    }
}
