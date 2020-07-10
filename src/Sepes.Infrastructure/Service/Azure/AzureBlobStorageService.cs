
using Azure;
using Azure.Storage;
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
using Sepes.Infrastructure.Interface;
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
        //readonly AzureSasTokenService _sasTokenService;

        public enum ImageFormat
        {
            bmp,
            jpg,
            jpeg,
            png
        }

        public AzureBlobStorageService(IConfiguration config)
        {
            this._config = config;
            this._connectionString = config["AzureStorageConnectionString"];

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

        public Response<bool> DeleteBlob(string fileName)
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

        public async Task<StudyDto> DecorateLogoUrlWithSAS(StudyDto studyDto)
        {
            var uriBuilder = await CreateUriBuilderWithSasToken();

            DecorateLogoUrlsWithSAS(uriBuilder, studyDto);

            return studyDto;
        }

        public async Task<IEnumerable<StudyListItemDto>> DecorateLogoUrlsWithSAS(IEnumerable<StudyListItemDto> studyDtos)
        {
            var uriBuilder = await CreateUriBuilderWithSasToken();

            foreach (var curDto in studyDtos)
            {
                DecorateLogoUrlsWithSAS(uriBuilder, curDto);             
            }

            return studyDtos;
        }


        void DecorateLogoUrlsWithSAS(UriBuilder uriBuilder, IHasLogoUrl studyDto)
        {
            if (!String.IsNullOrWhiteSpace(studyDto.LogoUrl))
            {
                uriBuilder.Path = string.Format("{0}/{1}", _containerName, studyDto.LogoUrl);
                studyDto.LogoUrl = uriBuilder.Uri.ToString();
            }
            else
            {
                studyDto.LogoUrl = null;
            }            
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
                var sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = _containerName,
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

        private string GetKeyValueFromConnectionString(string key)
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


        bool FileIsCorrectImageFormat(IFormFile file)
        {
            string suppliedFileName = file.FileName;
            string fileType = suppliedFileName.Split('.').Last();
            return !String.IsNullOrWhiteSpace(fileType) &&
                (  fileType.Equals(ImageFormat.png.ToString())
                || fileType.Equals(ImageFormat.jpeg.ToString())
                || fileType.Equals(ImageFormat.jpg.ToString())
                || fileType.Equals(ImageFormat.bmp.ToString())
                );
        }
    }
}
