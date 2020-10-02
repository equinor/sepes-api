using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceMonitoringService : IAzureResourceMonitoringService
    {
        readonly IConfiguration _config;
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;    
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IMapper _mapper;

        public AzureResourceMonitoringService(IConfiguration config, ILogger<AzureResourceMonitoringService> logger, IServiceProvider serviceProvider, ISandboxResourceService sandboxResourceService, IMapper mapper)
        {
            _config = config;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sandboxResourceService = sandboxResourceService;
            _mapper = mapper;
        }

        public async Task StartMonitoringSession()
        {
            var activeResources = await _sandboxResourceService.GetActiveResources();

            foreach (var curRes in activeResources)
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(curRes.ResourceType)) _logger.LogCritical($"Resource type field empty in DB for resource id: {curRes.Id}");

                    await GetAndLogProvisioningState(curRes);
                    await CheckAndUpdateTags(curRes);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Monitoring failed for resource id: {curRes.Id}");

                }
            }
            //TODO: Check for orphan resources in Azure (Tag and report). (Resources that Exists in Azure, but are marked as deleted in DB.)
            //await CheckForOrphanResources();
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
                    var managedByTagValue = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.MANAGED_BY);

                    // Read info used to create tags from resourceGroup in DB
                    // These tags should be checked with the ones in Azure.
                    var studyDto = _mapper.Map<StudyDto>(resource.Sandbox.Study);
                    var sandboxDto = _mapper.Map<SandboxDto>(resource.Sandbox);
                    var tagsFromDb = AzureResourceTagsFactory.CreateTags(managedByTagValue, studyDto.Name, studyDto, sandboxDto);

                    var tagsFromAzure = await serviceForResource.GetTagsAsync(resource.ResourceGroupName, resource.ResourceName);

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

                            if (!String.IsNullOrWhiteSpace(provisioningState))
                            {
                                //TODO: VERIFY THAT THIS WORKS AND THAT IT TAKES INNTO ACCOUNT RESOURCES THAT HAS BEEN RECENTLY DELETED(LIKE 5 minutes ago)

                                // TODO: Either remove Deleted tag in db or delete from Azure.
                                _logger.LogCritical($"Found orphan resource in Azure: Id: {resource.Id}, Name: {resource.ResourceName}");

                            }
                        }
                        catch (Exception)
                        {
                            //TODO: Handle not found exception, that would be a good sign
                            throw;
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Orphan check failed for resource: {resource.Id}");
                }
            }
        }
    }
}
