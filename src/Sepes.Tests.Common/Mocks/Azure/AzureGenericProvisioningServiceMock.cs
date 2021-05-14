using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Provisioning;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Common.Mocks.Azure
{
    public abstract class AzureGenericProvisioningServiceMock
    {
        protected readonly ILogger _logger;
        protected readonly string _resourceType;

        public AzureGenericProvisioningServiceMock(ILogger<AzureGenericProvisioningServiceMock> logger, string resourceType)
        {
            _logger = logger;
            _resourceType = resourceType;
        }

        protected void Log(string message)
        {
            _logger.LogInformation($"Azure mock service for {_resourceType}: {message}");
        }

        public Task<ResourceProvisioningResult> EnsureCreated(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            if (_resourceType == AzureResourceType.ResourceGroup)
            {
                Log($"Creating resource group: {parameters.ResourceGroupName}");
            }
            else
            {
                Log($"Creating resource {parameters.Name} in resource group {parameters.ResourceGroupName}");
            }

            throw new System.NotImplementedException();
        }

        public Task<ResourceProvisioningResult> Update(ResourceProvisioningParameters parameters, CancellationToken cancellationToken = default)
        {
            if (_resourceType == AzureResourceType.ResourceGroup)
            {
                Log($"Updating resource group: {parameters.ResourceGroupName}");
            }
            else
            {
                Log($"Updating resource {parameters.Name} in resource group {parameters.ResourceGroupName}");
            }

            throw new System.NotImplementedException();
        }

        public Task Delete(string resourceGroupName, string resourceName)
        {
            throw new System.NotImplementedException();
        }

        public Task<ResourceProvisioningResult> Delete(ResourceProvisioningParameters parameters)
        {
            if (_resourceType == AzureResourceType.ResourceGroup)
            {
                Log($"Deleting resource group: {parameters.ResourceGroupName}");
            }
            else
            {
                Log($"Updating resource {parameters.Name} in resource group {parameters.ResourceGroupName}");
            }

            throw new System.NotImplementedException();
        }

        public Task<ResourceProvisioningResult> GetSharedVariables(ResourceProvisioningParameters parameters)
        {
            if (_resourceType == AzureResourceType.ResourceGroup)
            {
                Log($"Returning shared variables for resource group: {parameters.ResourceGroupName}");
            }
            else
            {
                Log($"Returning shared variables for resource {parameters.Name} in resource group {parameters.ResourceGroupName}");
            }

            throw new System.NotImplementedException();
        }

        public Task<string> GetProvisioningState(string resourceGroupName)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            throw new System.NotImplementedException();
        }

        //ResourceProvisioningResult CreateCRUDResult(IVirtualMachine vm)
        //{
        //    var crudResult = ResourceProvisioningResultUtil.CreateResultFromIResource(vm);
        //    crudResult.CurrentProvisioningState = vm.Inner.ProvisioningState.ToString();
        //    return crudResult;
        //}

        //ResourceProvisioningResult CreateBasicProvisioningResult(string resourceGroupName, string resourceName)
        //{
        //    //Id, foreign name
        //    nameof//ResourceGroipma,e
        //}

        protected abstract void DecorateProvisioningResult(ResourceProvisioningResult result);
    }
}
