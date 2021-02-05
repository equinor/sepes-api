using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetFileService
    { 
        Task<List<BlobStorageItemDto>> AddFiles(int datasetId, List<IFormFile> files, CancellationToken cancellationToken = default);

        Task DeleteFileAsync(int datasetId, string fileName, CancellationToken cancellationToken = default);

        Task<List<BlobStorageItemDto>> GetFileListAsync(int datasetId, CancellationToken cancellationToken = default);

        Task<string> GetFileUploadUriBuilderWithSasTokenAsync(int datasetId, CancellationToken cancellationToken = default);
    }
}
