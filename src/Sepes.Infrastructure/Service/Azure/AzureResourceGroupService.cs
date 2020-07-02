using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading;
using Microsoft.Azure.Management.Fluent;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceGroupService
    {
        private readonly IAzure _azure;
        //TODO: Check old code, might be sometgoing here

        //Parameters to include
        //Region, Name (study + sandbox ++) //rg-study
        //Tags


        public string CreateResourceGroupName(string studyName, string sandboxName)
        {
            return $"rg-study-{studyName}-{sandboxName}";
        }

        public async Task<string> CreateResourceGroup(string studyName, string sandboxName, Region region, Dictionary<string, string> tags)
        {
            var resourceGroupName = CreateResourceGroupName(studyName, sandboxName);

            //TODO: Add tags, where to get?
            //TechnicalContact (Specified per sandbox?)
            //TechnicalContactEmail (Specified per sandbox?)
            //Sponsor
            //SponsorEmail
            var resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(region)
                    .WithTags(tags)
                    .CreateAsync();

            //return resource id from iresource objects
            return resourceGroup.Id;
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
