
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        readonly IConfiguration _config;
        readonly string _connectionString;
        readonly string _containerName = "studylogos";
        readonly AzureSasTokenService _sasTokenService;

        public enum ImageFormat
        {
            bmp,
            jpg,
            jpeg,
            png
        }

        public AzureBlobStorageService(IConfiguration config, AzureSasTokenService sasTokenService)
        {
            this._config = config;
            this._connectionString = config["AzureStorageConnectionString"];
            _sasTokenService = sasTokenService;
        }

        BlobContainerClient CreateBlobContainerClient()
        {
            return new BlobContainerClient(this._connectionString, _containerName);
        }

        BlobServiceClient CreateBlobServiceClient()
        {
            return new BlobServiceClient(this._connectionString);
        }

        public string UploadBlob(IFormFile blob)
        {
            if (!FileIsCorrectImageFormat(blob))
            {
                throw new ArgumentException("Blob has invalid filename or is not of type png, jpg or bmp.");
            }

            string suppliedFileName = blob.FileName;
            var blobContainerClient = CreateBlobContainerClient();
            string fileName = Guid.NewGuid().ToString("N") + suppliedFileName;
            var fileStream = blob.OpenReadStream();
            blobContainerClient.UploadBlob(fileName, fileStream);
            return fileName;

        }

        public Azure.Response<bool> DeleteBlob(string fileName)
        {
            var blobContainerClient = CreateBlobContainerClient();
            return blobContainerClient.DeleteBlobIfExists(fileName);
        }

        public async Task<byte[]> GetImageFromBlobAsync(string logoUrl)
        {
            var blobContainerClient = CreateBlobContainerClient();
            var blockBlobClient = blobContainerClient.GetBlockBlobClient(logoUrl);
            var memStream = new MemoryStream();
            await blockBlobClient.DownloadToAsync(memStream);
            return memStream.ToArray();
        }

        public async Task<IEnumerable<StudyListItemDto>> DecorateLogoUrlsWithSAS(IEnumerable<StudyListItemDto> studyDtos)
        {
            var uriBuilder = await CreateUriBuilderWithSasToken();

            foreach (var curDto in studyDtos)
            {
                uriBuilder.Path = string.Format("{0}/{1}", _containerName, curDto.LogoUrl);
                curDto.LogoUrl = uriBuilder.Uri.ToString();
            }

            return studyDtos;

        }


        public async Task<UriBuilder> CreateUriBuilderWithSasToken()
        {
            bool isDevelopmentStorage = this._connectionString == "UseDevelopmentStorage=true";

            var blobServiceClient = new BlobServiceClient(this._connectionString);

            var uriBuilder = new UriBuilder()
            {
                Scheme = "https",
                Host = string.Format("{0}.blob.core.windows.net", blobServiceClient.AccountName),

            };

            if (isDevelopmentStorage)
            {
                return uriBuilder;
            }
            else
            {
                var bajs = await _sasTokenService.CreateAdAuthenticatedClient();

                var tenantId = _config[ConfigConstants.TENANT_ID];
                var clientId = _config[ConfigConstants.AZ_CLIENT_ID];
                var clientSecret = _config[ConfigConstants.AZ_CLIENT_SECRET];
                var subscriptionId = _config[ConfigConstants.SUBSCRIPTION_ID];



                //var _credentials = new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscriptionId);

                //blobServiceClient = new BlobServiceClient(blobServiceClient.Uri, credential: _credentials);


             

                    var delegationKey = await bajs.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(7));

                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = _containerName,
                    Resource = "c",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(30)
                    
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasQueryParams = sasBuilder.ToSasQueryParameters(delegationKey, blobServiceClient.AccountName).ToString();

                uriBuilder.Query = sasQueryParams;

                return uriBuilder;
            }

        }



       

        bool FileIsCorrectImageFormat(IFormFile file)
        {
            string suppliedFileName = file.FileName;
            string fileType = suppliedFileName.Split('.').Last();
            if (!String.IsNullOrWhiteSpace(fileType)
                && (fileType.Equals(ImageFormat.png.ToString())
                || fileType.Equals(ImageFormat.jpeg.ToString())
                || fileType.Equals(ImageFormat.jpg.ToString())
                || fileType.Equals(ImageFormat.bmp.ToString()))
                )
            {
                return true;
            }
            return false;
        }
    }
}
