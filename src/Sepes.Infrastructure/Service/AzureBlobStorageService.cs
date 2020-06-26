using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using System.Runtime.CompilerServices;
using System.Linq;
using System.IO;
using Azure.Storage.Blobs.Specialized;

namespace Sepes.Infrastructure.Service
{
    class AzureBlobStorageService
    {
      // protected readonly string devUriSchema = "http://127.0.0.1:10000/<account-name>/<resource-path>";
      // protected readonly string devUri = "http://127.0.0.1:10000/devstoreaccount1/logos";
      // protected readonly string devEquinorUri = "http://127.0.0.1:10000/devstoreaccount1/logos/equinor_small.png";
      // protected readonly string prodUriSchema = "<http|https>://<account-name>.<service-name>.core.windows.net/<resource-path>";
      // protected readonly string StorageConnectionString = "UseDevelopmentStorage=true";
        private readonly string connectionString;
        private readonly string containerName = "logos";

        // Enum not in use yet..
        public enum ImageFormat
        {
            bmp,
            jpeg,
            gif,
            tiff,
            png,
            unknown
        }

        public AzureBlobStorageService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public string UploadBlob(IFormFile blob)
        {
            if (!FileIsCorrectImageFormat(blob))
            {
                throw new ArgumentException("Blob has invalid filename or is not of type png or jpg.");
            }
            string suppliedFileName = blob.FileName;
            BlobContainerClient blobContainerClient = new BlobContainerClient(this.connectionString, containerName);
            string fileName = Guid.NewGuid().ToString("N") + suppliedFileName;
            var fileStream = blob.OpenReadStream();
            blobContainerClient.UploadBlob(fileName, fileStream);
            return fileName;

        }

        public Azure.Response<bool> DeleteBlob(string fileName)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(this.connectionString, containerName);
            return blobContainerClient.DeleteBlobIfExists(fileName);
        }

        public async System.Threading.Tasks.Task<byte[]> GetImageFromBlobAsync(string logoUrl)
        {
            BlobContainerClient blobContainerClient = new BlobContainerClient(this.connectionString, containerName);
            var blobClient = blobContainerClient.GetBlockBlobClient(logoUrl);
            MemoryStream memStream = new MemoryStream();
            await blobClient.DownloadToAsync(memStream);
            return memStream.ToArray();
        }

        bool FileIsCorrectImageFormat(IFormFile file)
        {
            string suppliedFileName = file.FileName;
            string fileType = suppliedFileName.Split('.').Last();
            if (!String.IsNullOrWhiteSpace(fileType) && (fileType.Equals("png") || fileType.Equals("tiff") || fileType.Equals("jpeg") || fileType.Equals("jpg") || fileType.Equals("bmp")))
            {
                return true;
            }
            return false;
        }
    }
}
