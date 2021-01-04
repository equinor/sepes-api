using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Provisioning
{
    public class ResourceProvisioningResult
    {
       
        public string CurrentProvisioningState { get; set; }

        public string IdInTargetSystem { get; set; }

        public string NameInTargetSystem { get; set; }

        public IResource Resource { get; set; }

        public Microsoft.Rest.Azure.IResource NetworkResource { get; set; }

        public Dictionary<string, string> NewSharedVariables { get; set; } = new Dictionary<string, string>();
    }
}
