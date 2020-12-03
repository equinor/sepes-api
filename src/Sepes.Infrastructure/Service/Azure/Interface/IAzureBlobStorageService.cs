using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBlobStorageService
    {
        void SetConfigugrationKeyForConnectionString(string connectionStringConfigName);

        Task UploadFileToBlobContainer(string containerName, string blobName, IFormFile file);

        Task<int> DeleteFileFromBlobContainer(string containerName, string blobName);
        Task<FileStreamResult> DownloadFileFromBlobContainer(string containerName, string blobName, string fileName);
        UriBuilder CreateUriBuilderWithSasToken(string containerName);
        // Task<FileStreamResult> DownloadFile(string containerName,string blobName, string fileName);


        //Response<bool> DeleteBlob(string fileName);

        //string UploadBlob(IFormFile blob);
    }
}