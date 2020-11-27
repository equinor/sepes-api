using Sepes.Infrastructure.Dto.Interfaces;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxResourceLightDto : IHasLinkToExtSystem
    {  
        public string Name { get; set; }

        public string Type { get; set; }       

        public string Status { get; set; }

        public bool SandboxControlled { get; set; }

        public string LastKnownProvisioningState { get; set; }

        public string LinkToExternalSystem { get; set; }

        public string RetryLink { get; set; }

    }
}
