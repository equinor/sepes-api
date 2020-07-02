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
        private readonly string connectionString;
        private readonly string containerName = "logos";
        //private readonly string containerName = "studylogos";

        public enum ImageFormat
        {
            bmp,
            jpg,
            jpeg,
            png
        }

        public AzureBlobStorageService(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public string UploadBlob(IFormFile blob)
        {
            if (!FileIsCorrectImageFormat(blob))
            {
                throw new ArgumentException("Blob has invalid filename or is not of type png, jpg or bmp.");
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
