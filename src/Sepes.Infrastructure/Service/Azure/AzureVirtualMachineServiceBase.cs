using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Exceptions;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureVirtualMachineServiceBase : AzureServiceBase
    {      

        public AzureVirtualMachineServiceBase(IConfiguration config, ILogger logger)
            : base(config, logger)
        {
                  
        }

        protected async Task<IVirtualMachine> GetInternalAsync(string resourceGroupName, string resourceName)
        {
            var resource = await _azure.VirtualMachines.GetByResourceGroupAsync(resourceGroupName, resourceName);
            return resource;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            var resource = await GetInternalAsync(resourceGroupName, resourceName);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource.ProvisioningState;
        }

       
    }
}
