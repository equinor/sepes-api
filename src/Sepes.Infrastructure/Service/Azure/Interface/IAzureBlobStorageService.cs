using Azure;
using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBlobStorageService
    {
        IEnumerable<StudyListItemDto> DecorateLogoUrlsWithSAS(IEnumerable<StudyListItemDto> studyDtos);    

        void DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo);

        Response<bool> DeleteBlob(string fileName);
        Task<byte[]> GetImageFromBlobAsync(string logoUrl);
        string UploadBlob(IFormFile blob);
    }
}