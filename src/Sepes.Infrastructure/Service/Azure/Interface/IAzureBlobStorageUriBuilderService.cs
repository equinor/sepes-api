using Azure.Storage.Sas;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBlobStorageUriBuilderService
    {
        void SetConnectionParameters(string resourceGroupName, string accountName);
        void SetConnectionParameters(string connectionStringConfigName);     

      
        Task<UriBuilder> CreateUriBuilder(string containerName, string resourceType = "c", BlobContainerSasPermissions permission = BlobContainerSasPermissions.Read, int expiresOnMinutes = 10, CancellationToken cancellationToken = default);
    }
}
