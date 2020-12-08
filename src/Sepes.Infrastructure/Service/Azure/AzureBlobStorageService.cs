using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.Storage.Fluent;
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

    public class AzureBlobStorageService : AzureServiceBase, IAzureBlobStorageService
    {

        AzureBlobStorageConnectionDetails _connectionDetails;

        public AzureBlobStorageService(IConfiguration configuration, ILogger<AzureBlobStorageService> logger)
            : base(configuration, logger)
        {

        }

        public void SetConfigugrationKeyForConnectionString(string connectionStringConfigName)
        {
            _connectionDetails = AzureBlobStorageConnectionDetails.CreateUsingConnectionString(GetStorageConnectionString(connectionStringConfigName));
        }

        public void SetResourceGroupAndAccountName(string resourceGroupName, string accountName)
        {
            _connectionDetails = AzureBlobStorageConnectionDetails.CreateUsingResourceGroupAndAccountName(resourceGroupName, accountName);
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
                await blobClient.UploadAsync(stream, blobHttpHeader);
                stream.Close();
            }

            return await GetFileList(containerName, cancellationToken);
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
                    result.Add(new BlobStorageItemDto() { Name = curBlob.Name, ContentType = curBlob.Properties.ContentType, SizeInBytes = curBlob.Properties.ContentLength.HasValue ? curBlob.Properties.ContentLength.Value : 0 });
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return result;
        }

        //public async Task<FileStreamResult> DownloadFileFromBlobContainer(string containerName, string blobName, string fileName, CancellationToken cancellationToken = default)
        //{
        //    MemoryStream ms = new MemoryStream();

        //    var blobServiceClient = await GetBlobServiceClient();
        //    var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);


        //    var blobClient = blobContainerClient.GetBlobClient(blobName);
        //    await blobClient.DownloadToAsync(ms);

        //       var blob = container.GetBlobReference(blobName);

        //    await blob.DownloadToStreamAsync(ms);
        //    Stream blobStream = blob.OpenReadAsync().Result;
        //    var fileStream = new FileStreamResult(blobStream, blob.Properties.ContentType);
        //    fileStream.FileDownloadName = fileName;

        //    return fileStream;
        //}

        public async Task<int> DeleteFileFromBlobContainer(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var blobServiceClient = await GetBlobServiceClient();
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteAsync();

            return result.Status;
        }

        public async Task<UriBuilder> CreateUriBuilderWithSasToken(string containerName)
        {
            var accountName = await GetAccountNameFromConnectionString();

            if (String.IsNullOrWhiteSpace(accountName) == false)
            {
                var uriBuilder = new UriBuilder()
                {
                    Scheme = "https",
                    Host = string.Format("{0}.blob.core.windows.net", accountName)
                };

                if (_connectionDetails.IsDevelopmentStorage)
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
            if (String.IsNullOrWhiteSpace(_connectionDetails.ConnectionString))
            {
                throw new Exception("Connection String not set");
            }

            var settings = new Dictionary<string, string>();
            var splitted = _connectionDetails.ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nameValue in splitted)
            {
                var splittedNameValue = nameValue.Split(new char[] { '=' }, 2);
                settings.Add(splittedNameValue[0], splittedNameValue[1]);
            }

            return settings[key];
        }

        async Task<string> GetAccountNameFromConnectionString(CancellationToken cancellationToken = default)
        {
            var blobServiceClient = await GetBlobServiceClient(cancellationToken);

            return blobServiceClient.AccountName;
        }

        string GetStorageConnectionString(string nameOfConfig)
        {
            return _config[nameOfConfig];
        }

        async Task<BlobServiceClient> GetBlobServiceClient(CancellationToken cancellationToken = default)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_connectionDetails.ConnectionString) == false)
                {
                    return new BlobServiceClient(_connectionDetails.ConnectionString);
                }
                else
                {
                    string accessKey = await GetAccessKey(cancellationToken);

                    var credential = new StorageSharedKeyCredential(_connectionDetails.StorageAccountName, accessKey);

                    //Should have access through subscription? Or neet to get token/access key?
                    return new BlobServiceClient(new Uri($"https://{_connectionDetails.StorageAccountName}.blob.core.windows.net"), credential);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong when creating BlobServiceClient");
                throw new Exception($"Unable to create BlobServiceClient", ex);
            }
        }

        async Task<string> GetAccessKey(CancellationToken cancellationToken = default)
        {
            try
            {
                string accessKey = null;

                if (String.IsNullOrWhiteSpace(_connectionDetails.StorageAccountId) == false)
                {
                    accessKey = await GetAccessKey(_connectionDetails.StorageAccountId, cancellationToken);
                }
                else if (String.IsNullOrWhiteSpace(_connectionDetails.StorageAccountResourceGroup) == false && String.IsNullOrWhiteSpace(_connectionDetails.StorageAccountName) == false)
                {
                    accessKey = await GetAccessKey(_connectionDetails.StorageAccountResourceGroup, _connectionDetails.StorageAccountName, cancellationToken);
                }

                return accessKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed when getting access key");
                throw;
            }
        }

        async Task<string> GetAccessKey(string resourceGroup, string accountName, CancellationToken cancellationToken = default)
        {
            var storageAccount = await _azure.StorageAccounts.GetByResourceGroupAsync(resourceGroup, accountName, cancellationToken);
            return await GetKey(storageAccount, cancellationToken);
        }

        async Task<string> GetAccessKey(string storageAccountId, CancellationToken cancellationToken = default)
        {
            var storageAccount = await _azure.StorageAccounts.GetByIdAsync(storageAccountId, cancellationToken);
            return await GetKey(storageAccount, cancellationToken);
        }

        async Task<string> GetKey(IStorageAccount account, CancellationToken cancellationToken = default)
        {
            var keys = await account.GetKeysAsync(cancellationToken);

            foreach (var curKey in keys)
            {
                if (curKey.KeyName == "key1")
                {
                    return curKey.Value;
                }
            }

            return null;
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

    public class AzureBlobStorageConnectionDetails
    {

        public static AzureBlobStorageConnectionDetails CreateUsingConnectionString(string connectionString)
        {
            return new AzureBlobStorageConnectionDetails() { ConnectionString = connectionString };
        }

        public static AzureBlobStorageConnectionDetails CreateUsingResourceGroupAndAccountName(string resourceGroup, string accountName)
        {
            return new AzureBlobStorageConnectionDetails() { StorageAccountResourceGroup = resourceGroup, StorageAccountName = accountName };
        }

        public static AzureBlobStorageConnectionDetails CreateUsingAccountId(string accountId)
        {
            return new AzureBlobStorageConnectionDetails() { StorageAccountId = accountId };
        }

        public string ConnectionString { get; private set; }

        public string StorageAccountResourceGroup { get; private set; }

        public string StorageAccountId { get; private set; }
        public string StorageAccountName { get; private set; }

        public bool IsDevelopmentStorage
        {
            get
            {

                return String.IsNullOrWhiteSpace(ConnectionString) == false && ConnectionString == "UseDevelopmentStorage=true";


            }
        }
    }
}
