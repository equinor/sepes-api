namespace Sepes.Infrastructure.Dto
{
    public class SandboxResourceLightDto
    {  
        public string Name { get; set; }

        public string Type { get; set; }       

        public string Status { get; set; }

        public bool SandboxControlled { get; set; }

        public string LastKnownProvisioningState { get; set; }

        public string LinkToExternalSystem { get; set; }
        
    }
}
