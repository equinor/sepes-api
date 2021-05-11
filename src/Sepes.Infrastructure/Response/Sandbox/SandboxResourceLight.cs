using Sepes.Common.Dto.Interfaces;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Response.Sandbox
{
    public class SandboxResourceLight : IHasLinkToExtSystem
    {  
        public string Name { get; set; }

        public string Type { get; set; }       

        public string Status { get; set; }      

        public string LinkToExternalSystem { get; set; }

        public string RetryLink { get; set; }

        public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

}
}
