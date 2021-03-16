using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountAccessKeyService
    {
        Task<string> GetStorageAccountKey(string resourceGroup, string accountName, CancellationToken cancellationToken = default);
        Task<string> GetStorageAccountKey(string storageAccountId, CancellationToken cancellationToken = default);          
    }
}
