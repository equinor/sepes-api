using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceMonitoringService : ICloudResourceMonitoringService
    {
        readonly IServiceProvider _serviceProvider;
        readonly IConfiguration _config;
        readonly ILogger _logger;

        readonly ICloudResourceReadService _sandboxResourceService;
        readonly ICloudResourceUpdateService _sandboxResourceUpdateService;

        public CloudResourceMonitoringService(IServiceProvider serviceProvider, IConfiguration config, ILogger<CloudResourceMonitoringService> logger, ICloudResourceReadService sandboxResourceService, ICloudResourceUpdateService sandboxResourceUpdateService)
        {
            _serviceProvider = serviceProvider;

            _config = config;
            _logger = logger;

            _sandboxResourceService = sandboxResourceService;
            _sandboxResourceUpdateService = sandboxResourceUpdateService;
        }

       
        public async Task StartMonitoringSession()
        {
           
            _logger.LogInformation($"Monitoring provisioning state and tags");

            var monitoringDisabled = _config[ConfigConstants.DISABLE_MONITORING];

            if (!String.IsNullOrWhiteSpace(monitoringDisabled) && monitoringDisabled.ToLower() == "true")
            {
                _logger.LogWarning($"Monitoring is disabled, aborting!");
                return;
            }

            var activeResources = await _sandboxResourceService.GetAllActiveResources();

            foreach (var curRes in activeResources)
            { 
                try
                {
                    if (curRes.ResourceId == AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME)
                    {
                        if (curRes.Created.AddMinutes(15) < DateTime.UtcNow)
                        {
                            _logger.LogWarning(SepesEventId.MONITORING_NEW_RESOURCE_ID_NOT_SET);
                        }
                        else
                        {
                            _logger.LogInformation($"No valid foreign resource Id specified for {curRes.Id}. Foreign Id was {curRes.ResourceId}. Aborting monitoring");
                            continue;
                        }
                    }

                    if (curRes.ResourceName == AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME)
                    {
                        if (curRes.Created.AddMinutes(15) < DateTime.UtcNow)
                        {
                            _logger.LogWarning(SepesEventId.MONITORING_NEW_RESOURCE_NAME_NOT_SET);
                        }
                        else
                        {
                            _logger.LogInformation($"No valid name specified for {curRes.Id}. Name was {curRes.ResourceName}. Aborting monitoring");
                            continue;
                        }
                    }

                    if (String.IsNullOrWhiteSpace(curRes.ResourceType))
                    {
                        _logger.LogInformation($"Resource type field empty in DB for resource id: {curRes.Id}");

                        continue;
                    }

                    //First detect if monitoring should proceed
                    if (curRes.Operations.Count == 0)
                    {
                        if (curRes.Created.AddMinutes(5) < DateTime.UtcNow)
                        {
                            _logger.LogWarning(SepesEventId.MONITORING_NO_OPERATIONS);
                        }
                        else
                        {
                            _logger.LogWarning($"No operations found for resource {curRes.Id}. Aborting monitoring");
                            continue;
                        }
                    }

                    foreach (var curOperation in curRes.Operations)
                    {
                        if (curOperation.Status == CloudResourceOperationState.NEW || curOperation.Status == CloudResourceOperationState.IN_PROGRESS)
                        {
                            if (curOperation.Created.AddMinutes(5) < DateTime.UtcNow)
                            {
                                _logger.LogWarning(SepesEventId.MONITORING_OPERATION_FROZEN);
                            }
                            else
                            {
                                _logger.LogInformation($"Ongoing operation detected for resource {curRes.Id}. Aborting monitoring");
                                continue;
                            }
                        }
                    }

                    _logger.LogInformation($"Initial checks passed. Performing monitoring for resource {curRes.Id}.");

                    await GetAndLogProvisioningState(curRes);

                    if(curRes.LastKnownProvisioningState == CloudResourceProvisioningStates.SUCCEEDED)
                    {
                        await CheckAndUpdateTags(curRes);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Monitoring failed for resource id: {curRes.Id}");

                }
            }

            _logger.LogInformation($"Done monitoring provisioning state and tags");

            await CheckForOrphanResources();
        }

        //Fetches the provisioning state for the resource and write this on our record of the resource
        public async Task<string> GetProvisioningState(CloudResourceDto resource)
        {
            return await GetProvisioningState(resource.ResourceId, resource.ResourceType, resource.ResourceGroupName, resource.ResourceName);
        }

        //Fetches the provisioning state for the resource and write this on our record of the resource
        async Task<string> GetProvisioningState(CloudResource resource)
        {
            return await GetProvisioningState(resource.ResourceId, resource.ResourceType, resource.ResourceGroupName, resource.ResourceName);
        }

        async Task<string> GetProvisioningState(string resourceId, string resourceType, string resourceGroupName, string resourceName)
        {
            try
            {
                var serviceForResource = AzureResourceServiceResolver.GetServiceWithProvisioningState(_serviceProvider, resourceType);

                if (serviceForResource == null)
                {
                    _logger.LogCritical($"Service not found for Azure Resource Type: {resourceType}, for resource: {resourceName}");
                }
                else
                {
                    return await serviceForResource.GetProvisioningState(resourceGroupName, resourceName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Getting provisioning state failed for resource id: {resourceId}");
            }

            return null;

        }

        //Fetches the provisioning state for the resource and write this on our record of the resource
        async Task GetAndLogProvisioningState(CloudResource resource)
        {
            try
            {
                var provisioningState = await GetProvisioningState(resource);              

                await _sandboxResourceUpdateService.UpdateProvisioningState(resource.Id, provisioningState);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Checking provisioning state failed for resource id: {resource.Id}");
            }

        }

        // Checks Tags from Resource in Azure with information from db. 
        // Makes sure they are equal.
        async Task CheckAndUpdateTags(CloudResource resource)
        {
            try
            {
                var serviceForResource = AzureResourceServiceResolver.GetServiceWithTags(_serviceProvider, resource.ResourceType);

                if (serviceForResource == null)
                {
                    LogMonitoringError(resource, SepesEventId.MONITORING_NO_TAG_SERVICE, $"Could not resolve tag service for resource type: {resource.ResourceType}", critical: true);
                }
                else
                {
                    // Read info used to create tags from resourceGroup in DB
                    // These tags should be checked with the ones in Azure.            
                    var tagsFromDb = AzureResourceTagsFactory.TagStringToDictionary(resource.Tags);
                    var tagsFromAzure = await serviceForResource.GetTagsAsync(resource.ResourceGroupName, resource.ResourceName);

                    if (tagsFromDb != null && tagsFromDb.Count > 0 && tagsFromAzure == null)
                    {
                        _logger.LogWarning(SepesEventId.MONITORING_NO_TAGS, $"No tags found for resource {resource.Id}!");
                        return;
                    }

                    // Check against tags from resource in Azure.
                    // If different => update Tags and report difference to Study Owner?
                    foreach (var tag in tagsFromAzure)
                    {
                        //Do not check CreatedByMachine-tag, as this will be different from original.
                        if (!tag.Key.Equals("CreatedByMachine"))
                        {
                            if (!tagsFromDb.TryGetValue(tag.Key, out string dbValue))
                            {
                                // If Tag exists in Azure but not in tags generated from DB-data, report.
                                // Means that user has added tags themselves in Azure.
                                LogMonitoringError(resource, SepesEventId.MONITORING_MANUALLY_ADDED_TAGS, $"Tag {tag.Key} : {tag.Value} has been added after resource creation!");

                            }
                            else
                            {
                                // If Tag exists in Azure and Db but has different value in Azure
                                if (!tag.Value.Equals(dbValue))
                                {
                                    LogMonitoringError(resource, SepesEventId.MONITORING_INCORRECT_TAGS, $"Tag {tag.Key} : {tag.Value} does not match value from Sepes : {dbValue}");

                                    //Update tag in Azure to match DB-information.
                                    await serviceForResource.UpdateTagAsync(resource.ResourceGroupName, resource.ResourceName, new KeyValuePair<string, string>(tag.Key, dbValue));
                                    _logger.LogWarning($"Updated Tag: {tag.Key} from value: {tag.Value} => {dbValue}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMonitoringError(resource, SepesEventId.MONITORING_CRITICAL, $"Tag check/update failed", ex);
            }
        }


        public async Task CheckForOrphanResources()
        {
            _logger.LogInformation($"Looking for orphan resources");

            // Check that resources marked as deleted in db does not exist in Azure.
            var deletedResources = await _sandboxResourceService.GetDeletedResourcesAsync();
           
            foreach (var currentDeletedResource in deletedResources)
            {
              
                try
                {
                    var serviceForResource = AzureResourceServiceResolver.GetServiceWithProvisioningState(_serviceProvider, currentDeletedResource.ResourceType);

                    if (serviceForResource == null)
                    {
                        LogMonitoringError(currentDeletedResource, SepesEventId.MONITORING_NO_PROVISIONING_STATE_SERVICE, $"Could not resolve provisioning service for resource type: {currentDeletedResource.ResourceType}", critical: true);
                    }
                    else
                    {
                        try
                        {
                            var provisioningState = await serviceForResource.GetProvisioningState(currentDeletedResource.ResourceGroupName, currentDeletedResource.ResourceName);

                            if (!String.IsNullOrWhiteSpace(provisioningState))
                            {
                                LogMonitoringError(currentDeletedResource, SepesEventId.MONITORING_DELETED_RESOURCE_STILL_PRESENT_IN_CLOUD, $"Resource is deleted from Sepes, but still exists in cloud. Provisioning state: {provisioningState}");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.ToLower().Contains("could not be found") && !ex.Message.ToLower().Contains("not found"))
                            {
                                throw;
                            }

                            //Do nothing, resource not found
                        }

                    }
                }
                catch (Exception ex)
                {
                    LogMonitoringError(currentDeletedResource, SepesEventId.MONITORING_CRITICAL, $"Orphan check failed", ex);
                }
            }

            _logger.LogInformation($"Done looking for orphan resources");
        }

        void LogMonitoringError(CloudResource resource, string eventId, string messageSuffix, Exception ex = null, bool critical = false)
        {
            var fullErrorMessage = $"Resource {resource.Id} ({resource.ResourceName}): {messageSuffix}";
            LogMonitoringErrorInner(resource, eventId, fullErrorMessage, ex, critical);
        }       

        void LogMonitoringErrorInner(CloudResource resource, string eventId, string message, Exception ex = null, bool critical = false)
        {
            if (ex == null)
            {
                if (critical)
                {
                    _logger.LogCritical(eventId, message);
                }
                else
                {
                    _logger.LogWarning(eventId, message);
                }
            }
            else
            {
                _logger.LogCritical(eventId, ex, message);
            }
        }
    }
}
