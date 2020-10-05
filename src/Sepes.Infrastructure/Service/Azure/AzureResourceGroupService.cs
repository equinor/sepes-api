using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceGroupService : AzureServiceBase, IAzureResourceGroupService
    {
        readonly IMapper _mapper; 

        public AzureResourceGroupService(IConfiguration config, ILogger<AzureResourceGroupService> logger, IMapper mapper)
            :base(config, logger)
        {
            _mapper = mapper;
        }
      

        public async Task<AzureResourceGroupDto> Create(string resourceGroupName, Region region, Dictionary<string, string> tags)
        {
            IResourceGroup resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(region)
                    .WithTags(tags)
                    .CreateAsync();     
            
            return MapToDto(resourceGroup);
        }

        AzureResourceGroupDto MapToDto(IResourceGroup resourceGroup)
        {
            var mapped = _mapper.Map<AzureResourceGroupDto>(resourceGroup);
            mapped.ResourceGroupName = mapped.Name;

            return mapped;
        }

        public async Task<IResourceGroup> GetResourceAsync(string resourceGroupName)
        {
            var resource = await _azure.ResourceGroups.GetByNameAsync(resourceGroupName);
            return resource;
        }  

        //public async Task<bool> Exists(string resourceGroupName) => await Exists(resourceGroupName, resourceGroupName);

        public async Task<string> GetProvisioningState(string resourceGroupName)
        {
            var resource = await GetResourceAsync(resourceGroupName);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceGroupName);
            }
            
            return resource.ProvisioningState;
        }

        public async Task Delete(string resourceGroupName)
        {
            var resource = await GetResourceAsync(resourceGroupName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

            await _azure.ResourceGroups.DeleteByNameAsync(resourceGroupName);
        }

        public async Task<IPagedCollection<IResourceGroup>> GetResourceGroupsAsList()
        {
            var rg = await _azure.ResourceGroups.ListAsync();
            return rg;
        }

        public Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            return GetProvisioningState(resourceGroupName);
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var rg = await GetResourceAsync(resourceGroupName);
            return AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(rg.Tags);         
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var rg = await GetResourceAsync(resourceGroupName);   

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, rg.Tags);

            _ = await rg.Update().WithoutTag(tag.Key).ApplyAsync();
                _ = await rg.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }
    }
}
