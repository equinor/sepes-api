using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxWithCloudResourcesDto
    {
        public int SandboxId { get; set; }
        public string StudyName { get; set; }

        public string SandboxName { get; set; }

        public string ResourceGroupId { get { return ResourceGroup.ResourceId; } }

        public string ResourceGroupName { get { return ResourceGroup.ResourceGroupName; } }

        public Region Region { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public SandboxResourceDto ResourceGroup { get; set; }       

        public IStorageAccount DiagnosticsStorage { get; set; }

        //public INetworkSecurityGroup NetworkSecurityGroup { get; set; }

        public AzureVNetDto VNet { get; set; }
    }
}
