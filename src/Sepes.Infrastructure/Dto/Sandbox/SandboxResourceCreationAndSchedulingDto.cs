using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxResourceCreationAndSchedulingDto
    {
        public int SandboxId { get; set; }
        public string StudyName { get; set; }

        public string SandboxName { get; set; }

        public string ResourceGroupId { get { return ResourceGroup.ResourceId; } }

        public string ResourceGroupName { get { return ResourceGroup.ResourceGroupName; } }

        public string BatchId { get; set; }

        public Region Region { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public SandboxResourceDto ResourceGroup { get; set; }

        public SandboxResourceDto DiagnosticsStorage { get; set; }

        public SandboxResourceDto NetworkSecurityGroup { get; set; }

        public SandboxResourceDto Network { get; set; }

        public SandboxResourceDto Bastion { get; set; }
        public SandboxResourceCreationAndSchedulingDto()
        {

        }
      
    }
}
