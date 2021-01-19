using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxResourceCreationAndSchedulingDto
    {
        public int StudyId { get; set; }

        public int SandboxId { get; set; }
      
        public string StudyName { get; set; }

        public string SandboxName { get; set; }

        public string ResourceGroupId { get { return ResourceGroup.ResourceId; } }

        public string ResourceGroupName { get; set; }

        public string BatchId { get; set; }

        public Region Region { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public CloudResourceDto ResourceGroup { get; set; }

        public CloudResourceDto DiagnosticsStorage { get; set; }

        public CloudResourceDto NetworkSecurityGroup { get; set; }

        public CloudResourceDto Network { get; set; }

        public CloudResourceDto Bastion { get; set; }
        public SandboxResourceCreationAndSchedulingDto()
        {

        }
      
    }
}
