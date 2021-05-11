using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBlobStorageService
    {
        void SetConnectionParameters(string connectionStringConfigName);
        void SetConnectionParameters(string resourceGroupName, string accountName);

        Task<List<BlobStorageItemDto>> UploadFileToBlobContainer(string containerName, string blobName, IFormFile file, CancellationToken cancellationToken = default);

        Task<int> DeleteFileFromBlobContainer(string containerName, string blobName, CancellationToken cancellationToken = default);

        Task<List<BlobStorageItemDto>> GetFileList(string containerName, CancellationToken cancellationToken = default);

        Task EnsureContainerExist(string containerName, CancellationToken cancellationToken = default);
    }
}