using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks.Azure
{
    public class AzureResourceGroupServiceMock : IAzureResourceGroupService
    {
        readonly Dictionary<string, AzureResourceGroupDto> _ds;

       public AzureResourceGroupServiceMock()
        {
            _ds = new Dictionary<string, AzureResourceGroupDto>();
        }

        AzureResourceGroupDto GetOrThrow(string resourceGroupName)
        {

            if (_ds.ContainsKey(resourceGroupName))
            {
                return _ds[resourceGroupName];
            }
            else
            {
                throw new ArgumentException("Resource group does not exists", "resourceGroupName");
            }
        }

        public async Task<AzureResourceGroupDto> Create(string resourceGroupName, Region region, Dictionary<string, string> tags)
        {
            if (_ds.ContainsKey(resourceGroupName))
            {
                throw new ArgumentException("Resource group exists", "resourceGroupName");
            }

            var newResourceGroup = new AzureResourceGroupDto() { Id = Guid.NewGuid().ToString(), Name = resourceGroupName, ResourceGroupName = resourceGroupName, Region = region, ProvisioningState = "Success" };
            _ds.Add(resourceGroupName, newResourceGroup);

           return newResourceGroup;
        }

        public Task Delete(string resourceGroupName)
        {
            var existing = GetOrThrow(resourceGroupName);
            _ds[resourceGroupName] = null;
            return null;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName)
        {
            return GetOrThrow(resourceGroupName).ProvisioningState;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            return GetOrThrow(resourceGroupName).ProvisioningState;
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            return GetOrThrow(resourceGroupName).Tags;
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var newTagList = new Dictionary<string, string>();         

            var resourceGroup = GetOrThrow(resourceGroupName);        
            
            foreach(var curTag in resourceGroup.Tags)
            {
                if(curTag.Key == tag.Key)
                {
                    newTagList.Add(tag.Key, tag.Value);                
                    break;
                }
                else
                {
                    newTagList.Add(curTag.Key, curTag.Value);
                }              
            }

            resourceGroup.Tags = newTagList;            
        }

        public Task<CloudResourceCRUDResult> EnsureCreatedAndConfigured(CloudResourceCRUDInput parameters)
        {
            throw new NotImplementedException();
        }

        public Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters)
        {
            throw new NotImplementedException();
        }

        public Task<CloudResourceCRUDResult> GetSharedVariables(CloudResourceCRUDInput parameters)
        {
            throw new NotImplementedException();
        }

        public Task<CloudResourceCRUDResult> EnsureCreatedAndConfigured(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<CloudResourceCRUDResult> Update(CloudResourceCRUDInput parameters)
        {
            throw new NotImplementedException();
        }
    }
}
