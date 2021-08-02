using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Exceptions;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureVirtualMachineServiceBase : AzureSdkServiceBase
    {      

        public AzureVirtualMachineServiceBase(IConfiguration config, ILogger logger, IAzureCredentialService azureCredentialService)
            : base(config, logger, azureCredentialService)
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
