using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceGroupService : AzureServiceBase, IAzureResourceGroupService
    { 
        
        public AzureResourceGroupService(IConfiguration config, ILogger<AzureResourceGroupService> logger)
            :base(config, logger)
        {
        }

      

        //public async Task<IResourceGroup> CreateForStudy(string studyName, string sandboxName, Region region, Dictionary<string, string> tags)
        //{
        //    string resourceGroupName = AzureResourceNameUtil.ResourceGroup(sandboxName);

        //    //TODO: Add tags, where to get?
        //    //TechnicalContact (Specified per sandbox?)
        //    //TechnicalContactEmail (Specified per sandbox?)
        //    //Sponsor
        //    //SponsorEmail

        //    return await Create(resourceGroupName, region, tags);         
        //}

        public async Task<IResourceGroup> Create(string resourceGroupName, Region region, Dictionary<string, string> tags)
        {
            IResourceGroup resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(region)
                    .WithTags(tags)
                    .CreateAsync();     
            
            return resourceGroup;
        }

        public async Task<IResourceGroup> GetResourceAsync(string resourceGroupName)
        {
            var resource = await _azure.ResourceGroups.GetByNameAsync(resourceGroupName);
            return resource;
        }

        public async Task<bool> Exists(string resourceGroupName, string resourceName)
        {
            var resource = await GetResourceAsync(resourceGroupName);

            if (resource == null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Exists(string resourceGroupName) => await Exists(resourceGroupName, resourceGroupName);

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

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetTags(string resourceGroupName, string resourceName)
        {
            var rg = await GetResourceAsync(resourceGroupName);
            return rg.Tags;
        }

        public async Task UpdateTag(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var rg = await GetResourceAsync(resourceGroupName);
                _ = await rg.Update().WithoutTag(tag.Key).ApplyAsync();
                _ = await rg.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }
    }
}
