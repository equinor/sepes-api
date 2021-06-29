using Sepes.Common.Dto.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetFileService
    { 
        Task<List<BlobStorageItemDto>> GetFileListAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default);

        Task<string> GetFileUploadUriAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default);

        Task<string> GetFileDeleteUriAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default);
    }
}
