using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Common.Exceptions;

namespace Sepes.Azure.Service
{
    public class AzureStorageAccountBaseService : AzureServiceBase
    {
        public AzureStorageAccountBaseService(IConfiguration config, ILogger logger)
            : base(config, logger)
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
