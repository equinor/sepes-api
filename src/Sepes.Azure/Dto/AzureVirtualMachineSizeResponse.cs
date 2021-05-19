using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;

namespace Sepes.Azure.Dto
{
    public class AzureVirtualMachineSizeResponse
    {
        public IEnumerable<VirtualMachineSize> Value { get; set; }
    }
}
