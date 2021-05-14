using System.Collections.Generic;

namespace Sepes.Common.Dto.Provisioning
{
    public class ResourceProvisioningResult
    {       
        public string CurrentProvisioningState { get; set; }

        public string IdInTargetSystem { get; set; }

        public string NameInTargetSystem { get; set; }          

        public Dictionary<string, string> NewSharedVariables { get; set; } = new Dictionary<string, string>();
    }
}
