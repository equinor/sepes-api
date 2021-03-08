using Sepes.Infrastructure.Dto.Interfaces;

namespace Sepes.Infrastructure.Response.Sandbox
{
    public class SandboxResourceLight : IHasLinkToExtSystem
    {  
        public string Name { get; set; }

        public string Type { get; set; }       

        public string Status { get; set; }      

        public string LinkToExternalSystem { get; set; }

        public string RetryLink { get; set; }

    }
}
