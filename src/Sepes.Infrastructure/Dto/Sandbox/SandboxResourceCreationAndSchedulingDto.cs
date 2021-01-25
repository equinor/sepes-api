using Sepes.Infrastructure.Model;
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

        public string Region { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public CloudResource ResourceGroup { get; set; }

        public CloudResource DiagnosticsStorage { get; set; }

        public CloudResource NetworkSecurityGroup { get; set; }

        public CloudResource Network { get; set; }

        public CloudResource Bastion { get; set; }
        public SandboxResourceCreationAndSchedulingDto()
        {

        }
      
    }
}
