using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class AzureVirtualMachineSizeResponse
    {
        public IEnumerable<VirtualMachineSize> Value { get; set; }
    }
}
