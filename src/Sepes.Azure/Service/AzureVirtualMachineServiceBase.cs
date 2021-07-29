using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Sepes.Common.Exceptions;

namespace Sepes.Azure.Service
{
    public class AzureVirtualMachineServiceBase : AzureSdkServiceBase
    {      

        public AzureVirtualMachineServiceBase(IConfiguration config, ILogger logger)
            : base(config, logger)
        {
                  
        }

        protected async Task<IVirtualMachine> GetInternalAsync(string resourceGroupName, string resourceName, bool failIfNotFound = true)
        {
            var resource = await _azure.VirtualMachines.GetByResourceGroupAsync(resourceGroupName, resourceName);

            if (resource == null)
            {
                if (failIfNotFound)
                {
                    throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
                }
                else
                {
                    return null;
                }
            }

            return resource;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            var resource = await GetInternalAsync(resourceGroupName, resourceName, false);

            if (resource == null)
            {
                return null;
            }

            return resource.ProvisioningState;
        }

       
    }
}
