using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureResourceGroupService
    {
        Task<IResourceGroup> CreateResourceGroupForStudy(string studyName, string sandboxName, Region region, Dictionary<string, string> tags);

        Task<IResourceGroup> CreateResourceGroup(string resourceGroupName, Region region, Dictionary<string, string> tags);

        Task<bool> Exists(string resourceGroupName);
    }
}
