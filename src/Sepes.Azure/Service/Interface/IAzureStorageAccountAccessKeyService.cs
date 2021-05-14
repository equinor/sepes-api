using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureStorageAccountAccessKeyService
    {
        Task<string> GetStorageAccountKey(string resourceGroup, string accountName, CancellationToken cancellationToken = default);
        Task<string> GetStorageAccountKey(string storageAccountId, CancellationToken cancellationToken = default);          
    }
}
