using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureResourceMonitoringService
    {
        readonly IServiceProvider _serviceProvider;
        readonly ILogger _logger;
        readonly ISandboxResourceService _sandboxResourceService;

        public AzureResourceMonitoringService(ILogger logger, IServiceProvider serviceProvider, ISandboxResourceService sandboxResourceService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _sandboxResourceService = sandboxResourceService;
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

        async Task CheckAndUpdateTags(SandboxResource resource)
        {


        }
    }
}
