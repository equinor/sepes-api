﻿using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceGroupService : IAzureResourceGroupService
    {
        private readonly IAzure _azure;        

        public string CreateResourceGroupNameForStudy(string studyName, string sandboxName)
        {
            return $"rg-study-{studyName}-{sandboxName}";
        }

        public async Task<IResourceGroup> CreateResourceGroupForStudy(string studyName, string sandboxName, Region region, Dictionary<string, string> tags)
        {
            var resourceGroupName = CreateResourceGroupNameForStudy(studyName, sandboxName);

            //TODO: Add tags, where to get?
            //TechnicalContact (Specified per sandbox?)
            //TechnicalContactEmail (Specified per sandbox?)
            //Sponsor
            //SponsorEmail

            return await CreateResourceGroup(resourceGroupName, region, tags);

           
            var resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(region)
                    .WithTags(tags)
                    .CreateAsync();

            return resourceGroup;
        }

        public async Task<IResourceGroup> CreateResourceGroup(string resourceGroupName, Region region, Dictionary<string, string> tags)
        {           
            var resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(region)
                    .WithTags(tags)
                    .CreateAsync();

            

            //return resource id from iresource objects
            return resourceGroup;
        }

        public async Task<bool> Exists(string resourceGroupName)
        {
            return await _azure.ResourceGroups.ContainAsync(resourceGroupName);
        }      

        public async Task DeleteResourceGroup(string resourceGroupName)
        {
            //Cancelation token can be saved so the azure delete can be aborted. But has not been done in this use case.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            await _azure.ResourceGroups.BeginDeleteByNameAsync(resourceGroupName, token);
        }

    }
}