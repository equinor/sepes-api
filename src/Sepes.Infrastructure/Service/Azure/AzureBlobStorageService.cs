using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Storage;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util.Azure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{

    public class AzureBlobStorageService : AzureBlobStorageServiceBase, IAzureBlobStorageService
    {
        IAzureStorageAccountTokenService _azureStorageAccountTokenService;

        public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger, IAzureStorageAccountTokenService azureStorageAccountTokenService)
            : base(configuration, logger)
        {
            _azureStorageAccountTokenService = azureStorageAccountTokenService;
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

            byte[] fileBytes;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
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

        protected override async Task<string> GetStorageAccountKey(CancellationToken cancellationToken = default)
        {
            try
            {
                return await AzureStorageUtils.GetStorageAccountKey(_azureStorageAccountTokenService, _connectionParameters, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Storage Account Key");
                throw;
            }

        }
    }   
}
