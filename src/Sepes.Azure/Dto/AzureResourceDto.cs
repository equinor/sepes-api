using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;

namespace Sepes.Azure.Dto
{
    public class AzureResourceDto
    {
        public string Id { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string ProvisioningState { get; set; }

        public string ResourceGroupName { get; set; }

        public string Type { get; set; }

        public string Region { get; set; }

        public Dictionary<string, string> Tags { get; set; }
    }
}
