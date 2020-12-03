using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        readonly ILogger _logger;
        readonly IConfiguration _configuration;
        string _connectionString;

        public AzureBlobStorageService(ILogger<AzureBlobStorageService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void SetConfigugrationKeyForConnectionString(string connectionStringConfigName)
        {
            _connectionString = GetStorageConnectionString(connectionStringConfigName);
        }     

        public async Task UploadFileToBlobContainer(string containerName, string blobName, IFormFile file)
        {

            var blobServiceClient = new BlobServiceClient(_connectionString);
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
                await blobClient.UploadAsync(stream, blobHttpHeader);
                stream.Close();
            }
        }     

        public async Task<FileStreamResult> DownloadFileFromBlobContainer(string containerName, string blobName, string fileName)
        {
            MemoryStream ms = new MemoryStream();
            CloudStorageAccount.TryParse(_connectionString, out CloudStorageAccount storageAccount);

            var BlobClient = storageAccount.CreateCloudBlobClient();
            var container = BlobClient.GetContainerReference(containerName);

            var blob = container.GetBlobReference(blobName);

            await blob.DownloadToStreamAsync(ms);
            Stream blobStream = blob.OpenReadAsync().Result;
            var fileStream = new FileStreamResult(blobStream, blob.Properties.ContentType);
            fileStream.FileDownloadName = fileName;

            return fileStream;
        }

        public async Task<int> DeleteFileFromBlobContainer(string containerName, string blobName)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteAsync();

            return result.Status;
        }

        public UriBuilder CreateUriBuilderWithSasToken(string containerName)
        {
            bool isDevelopmentStorage = this._connectionString == "UseDevelopmentStorage=true";

            string accountName = null;

            if (GetAccountNameFromConnectionString(out accountName))
            {
                var uriBuilder = new UriBuilder()
                {
                    Scheme = "https",
                    Host = string.Format("{0}.blob.core.windows.net", accountName)
                };

                if (isDevelopmentStorage)
                {
                    return uriBuilder;
                }
                else
                {
                    var sasBuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = containerName,
                        Resource = "c",
                        StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                        ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)

                    };

                    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                    var credential = new StorageSharedKeyCredential(GetKeyValueFromConnectionString("AccountName"), GetKeyValueFromConnectionString("AccountKey"));

                    var sasToken = sasBuilder.ToSasQueryParameters(credential);

                    uriBuilder.Query = sasToken.ToString();

                    return uriBuilder;
                }
            }
            else
            {
                return null;
            }
        }

        string GetKeyValueFromConnectionString(string key)
        {
            var settings = new Dictionary<string, string>();
            var splitted = _connectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nameValue in splitted)
            {
                var splittedNameValue = nameValue.Split(new char[] { '=' }, 2);
                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            return settings[key];
        }

        bool GetAccountNameFromConnectionString(out string accountName)
        {
            BlobServiceClient blobServiceClient = null;

            if (CreateBlobServiceClient(out blobServiceClient))
            {
                accountName = blobServiceClient.AccountName;
                return true;
            }

            accountName = null;
            return false;
        }

        bool CreateBlobServiceClient(out BlobServiceClient client)
        {
            client = null;

            try
            {
                client = new BlobServiceClient(_connectionString);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create new BlobServiceClient");
                return false;
            }
        }

        private string GetStorageConnectionString(string nameOfConfig)
        {
            return _configuration[nameOfConfig];
        }

        //public async Task<List<FileStreamResult>> DownloadFileFromBlobContainer(string connectionString, string containerName, List<BlobFileName> blobfiles)
        //{
        //    var fileStreams = new List<FileStreamResult>();
        //    CloudStorageAccount.TryParse(_connectionString, out CloudStorageAccount storageAccount);

        //    var BlobClient = storageAccount.CreateCloudBlobClient();
        //    var container = BlobClient.GetContainerReference(containerName);

        //    foreach (var file in blobfiles)
        //    {
        //        MemoryStream ms = new MemoryStream();

        //        var blob = container.GetBlobReference(file.BlobName);

        //        await blob.DownloadToStreamAsync(ms);
        //        Stream blobStream = blob.OpenReadAsync().Result;
        //        var fileStream = new FileStreamResult(blobStream, blob.Properties.ContentType);
        //        fileStream.FileDownloadName = file.FileName;

        //        fileStreams.Add(fileStream);
        //    }
        //    return fileStreams;
        //}
    }
}
