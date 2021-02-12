using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureStorageAccountBaseService : AzureServiceBase
    {
       

        public AzureStorageAccountBaseService(IConfiguration config, ILogger logger)
            : base(config, logger)
        {
         
        }

         

        public async Task<IStorageAccount> GetResourceAsync(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default)
        {
            var resource = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroupName, resourceName, cancellationToken);
            return resource;
        }

        public async Task<IStorageAccount> GetResourceOrThrowAsync(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName, cancellationToken);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource;
        }
    }
}
