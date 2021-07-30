using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureStorageAccountBaseService : AzureSdkServiceBase
    {
        public AzureStorageAccountBaseService(IConfiguration config, ILogger logger, IAzureCredentialService azureCredentialService)
            : base(config, logger, azureCredentialService)
        {
         
        }
         

        public async Task<IStorageAccount> GetResourceAsync(string resourceGroupName, string resourceName, bool failIfNotFound = true, CancellationToken cancellationToken = default)
        {
            var resource = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroupName, resourceName, cancellationToken);

            if (resource == null && failIfNotFound)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource;
        }      
    }
}
