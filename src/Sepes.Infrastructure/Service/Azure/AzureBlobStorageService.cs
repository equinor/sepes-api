
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        readonly ILogger _logger;
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

        public AzureBlobStorageService(IConfiguration config, ILogger<AzureBlobStorageService> logger)
        {
            this._logger = logger;
            this._config = config;
            this._connectionString = config[ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING];

        }

        bool CreateBlobContainerClient(out BlobContainerClient client)
        {
            client = null;

            try
            {
                client = new BlobContainerClient(this._connectionString, _containerName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create new BlobContainerClient");
                return false;
            }
           
        }

        bool CreateBlobServiceClient(out BlobServiceClient client)
        {
            client = null;

            try
            {
                client = new BlobServiceClient(this._connectionString);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create new BlobServiceClient");
                return false;
            }
        }


        public string UploadBlob(IFormFile blob)
        {
            if (!FileIsCorrectImageFormat(blob))
            {
                throw new ArgumentException("Blob has invalid filename or is not of type png, jpg or bmp.");
            }

            string suppliedFileName = blob.FileName;

            BlobContainerClient blobContainerClient;

            if(CreateBlobContainerClient(out blobContainerClient))
            {               
                string fileName = Guid.NewGuid().ToString("N") + suppliedFileName;
                var fileStream = blob.OpenReadStream();
                blobContainerClient.UploadBlob(fileName, fileStream);
                return fileName;
            }

            throw new Exception("File upload failed. Unable to crate connection to file storage");                 

        }

        public Response<bool> DeleteBlob(string fileName)
        {
            BlobContainerClient blobContainerClient;

            if (CreateBlobContainerClient(out blobContainerClient))
            {
                return blobContainerClient.DeleteBlobIfExists(fileName);
            }

            throw new Exception("File upload failed. Unable to crate connection to file storage");

        }

        public async Task<byte[]> GetImageFromBlobAsync(string logoUrl)
        {
            BlobContainerClient blobContainerClient;

            if (CreateBlobContainerClient(out blobContainerClient))
            {
                var blockBlobClient = blobContainerClient.GetBlockBlobClient(logoUrl);
                var memStream = new MemoryStream();
                await blockBlobClient.DownloadToAsync(memStream);
                return memStream.ToArray();
            }

            throw new Exception("File upload failed. Unable to crate connection to file storage");
        }

        public void DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo)
        {
            var uriBuilder = CreateUriBuilderWithSasToken();

            if(uriBuilder == null)
            {
                _logger.LogError("Unable to decorate logo urls");
            }
            else
            {
                DecorateLogoUrlsWithSAS(uriBuilder, hasLogo);
            } 
        }

        public IEnumerable<StudyListItemDto> DecorateLogoUrlsWithSAS(IEnumerable<StudyListItemDto> studyDtos)
        {
            var uriBuilder = CreateUriBuilderWithSasToken();

            if (uriBuilder == null)
            {
                _logger.LogError("Unable to decorate logo urls");
            }
            else
            {
                foreach (var curDto in studyDtos)
                {
                    DecorateLogoUrlsWithSAS(uriBuilder, curDto);
                }
            }          

            return studyDtos;
        }


        void DecorateLogoUrlsWithSAS(UriBuilder uriBuilder, IHasLogoUrl hasLogo)
        {
            if (!String.IsNullOrWhiteSpace(hasLogo.LogoUrl))
            {
                uriBuilder.Path = string.Format("{0}/{1}", _containerName, hasLogo.LogoUrl);
                hasLogo.LogoUrl = uriBuilder.Uri.ToString();
            }
            else
            {
                hasLogo.LogoUrl = null;
            }            
        }      

        UriBuilder CreateUriBuilderWithSasToken()
        {
            bool isDevelopmentStorage = this._connectionString == "UseDevelopmentStorage=true";

            BlobServiceClient blobServiceClient = null;
            
            if(CreateBlobServiceClient(out blobServiceClient))
            {
                var uriBuilder = new UriBuilder()
                {
                    Scheme = "https",
                    Host = string.Format("{0}.blob.core.windows.net", blobServiceClient.AccountName)
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
            else
            {
                return null;
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
