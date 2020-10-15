using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceMonitoringService : ISandboxResourceMonitoringService
    {
        readonly IConfiguration _config;
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;    
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IMapper _mapper;

        public SandboxResourceMonitoringService(IConfiguration config, ILogger<SandboxResourceMonitoringService> logger, IServiceProvider serviceProvider, ISandboxResourceService sandboxResourceService, IMapper mapper)
        {
            _config = config;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sandboxResourceService = sandboxResourceService;
            _mapper = mapper;
        }

        public async Task StartMonitoringSession()
        {
            _logger.LogInformation($"Monitoring provisioning state and tags");

            var monitoringDisabled = _config[ConfigConstants.DISABLE_MONITORING];

            if(!String.IsNullOrWhiteSpace(monitoringDisabled) && monitoringDisabled.ToLower() == "true")
            {
                _logger.LogWarning($"Monitoring is disabled, aborting!");
                return;
            }

            var activeResources = await _sandboxResourceService.GetActiveResources();

            foreach (var curRes in activeResources)
            {

                try
                {  
                    if (curRes.ResourceId == AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_NAME)
                    {
                        _logger.LogInformation($"No valid foreign resource Id specified for {curRes.Id}. Foreign Id was {curRes.ResourceId}. Aborting monitoring");
                        continue;
                    }

                    if (curRes.ResourceName == AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_NAME)
                    {
                        _logger.LogInformation($"No valid name specified for {curRes.Id}. Name was {curRes.ResourceName}. Aborting monitoring");
                        continue;
                    }

                    if (String.IsNullOrWhiteSpace(curRes.ResourceType))
                    {
                        _logger.LogInformation($"Resource type field empty in DB for resource id: {curRes.Id}");
                        continue;
                    }

                    //First detect if monitoring should proceed
                    if (curRes.Operations.Count == 0)
                    {
                        _logger.LogWarning($"No operations found for resource {curRes.Id}. Aborting monitoring");
                        continue;
                    }

                    foreach(var curOperations in curRes.Operations)
                    {
                        if(curOperations.Status == CloudResourceOperationState.NOT_STARTED || curOperations.Status == CloudResourceOperationState.IN_PROGRESS)
                        {
                            _logger.LogInformation($"Ongoing operation detected for resource {curRes.Id}. Aborting monitoring");
                            continue;
                        }
                    }                

                    _logger.LogInformation($"Initial checks passed. Performing monitoring for resource {curRes.Id}.");

                    await GetAndLogProvisioningState(curRes);
                    await CheckAndUpdateTags(curRes);
                    
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
        public async Task<string> GetProvisioningState(SandboxResourceDto resource)
        {
            return await GetProvisioningState(resource.ResourceId, resource.ResourceType, resource.ResourceGroupName, resource.ResourceName);
        }

        //Fetches the provisioning state for the resource and write this on our record of the resource
        async Task<string> GetProvisioningState(SandboxResource resource)
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
        async Task GetAndLogProvisioningState(SandboxResource resource)
        {
            try
            {
                var provisioningState = await GetProvisioningState(resource);

                if (String.IsNullOrWhiteSpace(provisioningState))
                {
                    provisioningState = "Provisioning state was empty";
                }

                await _sandboxResourceService.UpdateProvisioningState(resource.Id, provisioningState);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Checking provisioning state failed for resource id: {resource.Id}");
            }

        }

        // Checks Tags from Resource in Azure with information from db. 
        // Makes sure they are equal.
        async Task CheckAndUpdateTags(SandboxResource resource)
        {
            try
            {
                var serviceForResource = AzureResourceServiceResolver.GetServiceWithTags(_serviceProvider, resource.ResourceType);

                if (serviceForResource == null)
                {
                    _logger.LogCritical($"Service not found for Azure Resource Type: {resource.ResourceType}, for resource: {resource.ResourceName}");
                }
                else
                {                 

                    // Read info used to create tags from resourceGroup in DB
                    // These tags should be checked with the ones in Azure.
                    var studyDto = _mapper.Map<StudyDto>(resource.Sandbox.Study);
                    var sandboxDto = _mapper.Map<SandboxDto>(resource.Sandbox);
                    var tagsFromDb = AzureResourceTagsFactory.CreateTags(_config, studyDto, sandboxDto);

                    var tagsFromAzure = await serviceForResource.GetTagsAsync(resource.ResourceGroupName, resource.ResourceName);

                    if(tagsFromAzure == null)
                    {
                        _logger.LogWarning($"No tags found for resource {resource.Id}!");
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
                                _logger.LogWarning($"Tag {tag.Key} : {tag.Value} has been added after resource creation!");
                                //TODO: Proper report!
                            }
                            else
                            {
                                // If Tag exists in Azure and Db but has different value in Azure
                                if (!tag.Value.Equals(dbValue))
                                {
                                    //Report
                                    _logger.LogWarning($"Tag {tag.Key} : {tag.Value} does not match Db-info: {tag.Key} : {dbValue}");
                                    //Update tag in Azure to match DB-information.
                                    await serviceForResource.UpdateTagAsync(resource.ResourceGroupName, resource.ResourceName, new KeyValuePair<string, string>(tag.Key, dbValue));
                                    _logger.LogInformation($"Updated Tag: {tag.Key} from value: {tag.Value} => {dbValue}");
                                    //TODO: Proper report!
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Tag check/update failed for resource id: {resource.Id}");
            }
        }

        public async Task CheckForOrphanResources()
        {
            _logger.LogInformation($"Looking for orphan resources");

            // Check that resources marked as deleted in db does not exist in Azure.
            var deletedResources = await _sandboxResourceService.GetDeletedResourcesAsync();

            foreach (var resource in deletedResources)
            {
                try
                {                   

                    var serviceForResource = AzureResourceServiceResolver.GetServiceWithProvisioningState(_serviceProvider, resource.ResourceType);

                    if (serviceForResource == null)
                    {
                        _logger.LogCritical($"Service not found for Azure Resource Type: {resource.ResourceType}, for resource: {resource.ResourceName}");
                    }
                    else
                    {
                        try
                        {
                            var provisioningState = await serviceForResource.GetProvisioningState(resource.ResourceGroupName, resource.ResourceName);

                            if (String.IsNullOrWhiteSpace(provisioningState) == false)
                            { 
                                // TODO: What to do here in addition to logging? DO NOT REMOVE DELETE MARK FROM RESOURCE. Delete from Azure? Tag resource?
                                _logger.LogCritical($"Found orphan resource in Azure: Id: {resource.Id}, Name: {resource.ResourceName}");

                            }
                        }
                        catch (Exception ex)
                        {
                            if(ex.Message.ToLower().Contains("could not be found") == false && ex.Message.ToLower().Contains("not found") == false)
                            {
                                throw;
                               
                            }

                            //Do nothing, resource not found
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Orphan check failed for resource: {resource.Id}");
                }
            }

            _logger.LogInformation($"Done looking for orphan resources");
        }
    }
}
