using Azure;
using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureBlobStorageService
    {
        Task<IEnumerable<StudyListItemDto>> DecorateLogoUrlsWithSAS(IEnumerable<StudyListItemDto> studyDtos);    

        Task<StudyDto> DecorateLogoUrlWithSAS(StudyDto studyDto);

        Response<bool> DeleteBlob(string fileName);
        Task<byte[]> GetImageFromBlobAsync(string logoUrl);
        string UploadBlob(IFormFile blob);
    }
}