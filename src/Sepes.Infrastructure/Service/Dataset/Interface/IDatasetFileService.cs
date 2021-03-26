using Sepes.Infrastructure.Dto.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetFileService
    { 
        Task<List<BlobStorageItemDto>> GetFileListAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default);

        Task<string> GetFileUploadUriBuilderWithSasTokenAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default);

        Task<string> GetFileDeleteUriBuilderWithSasTokenAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default);
    }
}
