using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountTokenService
    {
        void SetConnectionParameters(string resourceGroupName, string accountName);
        void SetConnectionParameters(string connectionStringConfigName);

        Task<string> GetStorageAccountKey(string resourceGroup, string accountName, CancellationToken cancellationToken = default);
        Task<string> GetStorageAccountKey(string storageAccountId, CancellationToken cancellationToken = default);        

        Task<Uri> GetSasKey(string containerName = "files", CancellationToken cancellationToken = default);
        Task<UriBuilder> CreateFileDownloadUriBuilder(string containerName, CancellationToken cancellationToken = default);
        Task<UriBuilder> CreateFileUploadUriBuilder(string containerName, CancellationToken cancellationToken = default);
    }
}
