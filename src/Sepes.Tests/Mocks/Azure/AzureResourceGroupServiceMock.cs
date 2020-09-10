using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetTags(string resourceGroupName, string resourceName)
        {
            return GetOrThrow(resourceGroupName).Tags;
        }

        public async Task UpdateTag(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var newTagList = new List<KeyValuePair<string, string>>();         

            var resourceGroup = GetOrThrow(resourceGroupName);        
            
            foreach(var curTag in resourceGroup.Tags)
            {
                if(curTag.Key == tag.Key)
                {
                    newTagList.Add(curTag);                
                    break;
                }
                else
                {
                    newTagList.Add(curTag);
                }              
            }

            resourceGroup.Tags = newTagList;
        }
    }
}
