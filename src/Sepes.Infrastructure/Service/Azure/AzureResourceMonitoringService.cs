using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceMonitoringService
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IMapper _mapper;

        public AzureResourceMonitoringService(ILogger logger, IServiceProvider serviceProvider, ISandboxResourceService sandboxResourceService, IMapper mapper)
        {
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
                    if (String.IsNullOrWhiteSpace(curRes.ResourceType))
                    {
                        _logger.LogCritical($"Resource type field empty in DB for resource id: {curRes.Id}");
                    }

                    await GetAndLogProvisioningState(curRes);
                    await CheckAndUpdateTags(curRes);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, $"Monitoring failed for resource id: {curRes.Id}");

                }
            }
        }

        //Fetches the provisioning state for the resource and write this on our record of the resource
        async Task GetAndLogProvisioningState(SandboxResource resource)
        {
            try
            {
                var serviceForResource = AzureResourceServiceResolver.GetServiceWithProvisioningState(_serviceProvider, resource.ResourceType);

                if (serviceForResource == null)
                {
                    _logger.LogCritical($"Service not found for Azure Resource Type: {resource.ResourceType}");
                }
                else
                {
                    var provisioningState = await serviceForResource.GetProvisioningState(resource.ResourceGroupName, resource.ResourceName);

                    if (String.IsNullOrWhiteSpace(provisioningState))
                    {
                        provisioningState = "Provisioning state was empty";
                    }

                    await _sandboxResourceService.UpdateProvisioningState(resource.Id, provisioningState);
                }
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
                    _logger.LogCritical($"Service not found for Azure Resource Type: {resource.ResourceType}");
                }
                else
                {
                    // Read info used to create tags from resourceGroup in DB
                    // These tags should be checked with the ones in Azure.
                    var studyDto = _mapper.Map<StudyDto>(resource.Sandbox.Study);
                    var sandboxDto = _mapper.Map<SandboxDto>(resource.Sandbox);
                    var tagsFromDb = AzureResourceTagsFactory.CreateTags(studyDto.Name, studyDto, sandboxDto);

                    var tagsFromAzure = await serviceForResource.GetTags(resource.ResourceGroupName, resource.ResourceName);

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
                                    _logger.LogWarning($"Tag {tag.Key} : {tag.Value} does not match Db-info {tag.Key} : {dbValue}");
                                    //Update tag in Azure to match DB-information.
                                    await serviceForResource.UpdateTag(resource.ResourceGroupName, resource.ResourceName, new KeyValuePair<string, string>(tag.Key, dbValue));
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
    }
}
