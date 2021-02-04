using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBlobStorageService
    {
        void SetResourceGroupAndAccountName(string resourceGroupName, string accountName);
        void SetConfigugrationKeyForConnectionString(string connectionStringConfigName);

        Task<List<BlobStorageItemDto>> UploadFileToBlobContainer(string containerName, string blobName, IFormFile file, CancellationToken cancellationToken = default);

        Task<int> DeleteFileFromBlobContainer(string containerName, string blobName, CancellationToken cancellationToken = default);
        //Task<FileStreamResult> DownloadFileFromBlobContainer(string containerName, string blobName, string fileName, CancellationToken cancellationToken = default);
        Task<UriBuilder> CreateUriBuilderWithSasToken(string containerName);
        Task<List<BlobStorageItemDto>> GetFileList(string containerName, CancellationToken cancellationToken = default);

        Task<Uri> GetSasKey(string containerName = "files", CancellationToken cancellationToken = default);

        // Task<FileStreamResult> DownloadFile(string containerName,string blobName, string fileName);


        //Response<bool> DeleteBlob(string fileName);

        //string UploadBlob(IFormFile blob);
    }
}