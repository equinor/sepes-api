using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Storage;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{

    public class AzureBlobStorageService : AzureBlobStorageServiceBase, IAzureBlobStorageService
    {  
        public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger, IAzureStorageAccountAccessKeyService azureStorageAccountAccessKeyService)
            : base(configuration, logger, azureStorageAccountAccessKeyService)
        {
           
        }              

        public async Task<List<BlobStorageItemDto>> GetFileList(string containerName, CancellationToken cancellationToken = default)
        {
            var result = new List<BlobStorageItemDto>();
            var blobServiceClient = await GetBlobServiceClient();
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await blobContainerClient.CreateIfNotExistsAsync();

            var blobAsyncPageable = blobContainerClient.GetBlobsAsync().WithCancellation(cancellationToken);
            var enumerator = blobAsyncPageable.GetAsyncEnumerator();

            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var curBlob = enumerator.Current;
                    result.Add(new BlobStorageItemDto() { Name = curBlob.Name, ContentType = curBlob.Properties.ContentType, Size = curBlob.Properties.ContentLength.HasValue ? curBlob.Properties.ContentLength.Value : 0 });
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return result;
        }

        public async Task<List<BlobStorageItemDto>> UploadFileToBlobContainer(string containerName, string blobName, IFormFile file, CancellationToken cancellationToken = default)
        {
            var blobServiceClient = await GetBlobServiceClient();
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(blobName);

            var blobHttpHeader = new BlobHttpHeaders();

            blobHttpHeader.ContentType = file.ContentType;       

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);              
            }

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, cancellationToken: cancellationToken, httpHeaders: blobHttpHeader);
                stream.Close();
            }

            return await GetFileList(containerName, cancellationToken);
        }
          

        public async Task<int> DeleteFileFromBlobContainer(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var blobServiceClient = await GetBlobServiceClient();
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteAsync();

            return result.Status;
        }

        async Task<BlobServiceClient> GetBlobServiceClient(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(_connectionParameters.ConnectionString))
                {
                    return new BlobServiceClient(_connectionParameters.ConnectionString);
                }
                else
                {
                    string accessKey = await GetStorageAccountKey(cancellationToken);

                    var credential = new StorageSharedKeyCredential(_connectionParameters.StorageAccountName, accessKey);
                  
                    return new BlobServiceClient(new Uri($"https://{_connectionParameters.StorageAccountName}.blob.core.windows.net"), credential);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong when creating BlobServiceClient");
                throw new Exception($"Unable to connect to Azure Storage Account", ex);
            }
        }     

        public async Task EnsureContainerExist(string containerName, CancellationToken cancellationToken = default)
        {
            var blobServiceClient = await GetBlobServiceClient();
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync();
        }
    }   
}
