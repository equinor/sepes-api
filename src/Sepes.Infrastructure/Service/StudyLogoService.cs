using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyLogoService : IStudyLogoService
    {
        readonly string _containerName = "studylogos";

        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IAzureBlobStorageService _blobService;    

        public StudyLogoService(ILogger<StudyLogoService> logger, SepesDbContext db, IMapper mapper, IUserService userService, IAzureBlobStorageService blobService)
        {
            _logger = logger;          
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _blobService = blobService;

            _blobService.SetConfigugrationKeyForConnectionString(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
        }

        public void DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo)
        {
            var uriBuilder = _blobService.CreateUriBuilderWithSasToken(_containerName);

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
            var uriBuilder = _blobService.CreateUriBuilderWithSasToken(_containerName);

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
      
        
        public async Task<StudyDetailsDto> AddLogoAsync(int studyId, IFormFile studyLogo)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Update_Metadata, true);

            if (!FileIsCorrectImageFormat(studyLogo))
            {
                throw new ArgumentException("Blob has invalid filename or is not of type png, jpg or bmp.");
            }

            string uniqueFileName = Guid.NewGuid().ToString("N") + studyLogo.FileName;

            await _blobService.UploadFileToBlobContainer(_containerName, uniqueFileName, studyLogo);

            string oldFileName = studyFromDb.LogoUrl;

            studyFromDb.LogoUrl = uniqueFileName;

            await _db.SaveChangesAsync();

            if (!String.IsNullOrWhiteSpace(oldFileName))
            {
                _ = await _blobService.DeleteFileFromBlobContainer(_containerName, oldFileName);
            }

            return _mapper.Map<StudyDetailsDto>(studyFromDb);
        }

        //public async Task<byte[]> GetImageFromBlobAsync(string logoUrl)
        //{
        //    BlobContainerClient blobContainerClient;

        //    if (CreateBlobContainerClient(out blobContainerClient))
        //    {
        //        var blockBlobClient = blobContainerClient.GetBlockBlobClient(logoUrl);
        //        var memStream = new MemoryStream();
        //        await blockBlobClient.DownloadToAsync(memStream);
        //        return memStream.ToArray();
        //    }

        //    throw new Exception("File upload failed. Unable to crate connection to file storage");
        //}

        //public async Task<LogoResponseDto> GetLogoAsync(int studyId)
        //{
        //    try
        //    {
        //        var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Read, false);
        //        var response = new LogoResponseDto() { LogoUrl = studyFromDb.LogoUrl, LogoBytes = await _blobService.Do(studyFromDb.LogoUrl) };

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Unable to get logo for Study {studyId}");
        //        return null;
        //    }
        //}

        public async Task DeleteAsync(Study study)
        {
            if (!String.IsNullOrWhiteSpace(study.LogoUrl))
            {
                _ = await _blobService.DeleteFileFromBlobContainer(_containerName, study.LogoUrl);
            }
        }

        async Task<Study> GetStudyByIdAsync(int studyId, UserOperation userOperation, bool withIncludes)
        {
            return await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, userOperation, withIncludes);
        }

        bool FileIsCorrectImageFormat(IFormFile file)
        {
            string suppliedFileName = file.FileName;
            string fileType = suppliedFileName.Split('.').Last();
            return !String.IsNullOrWhiteSpace(fileType) &&
                (fileType.Equals(ImageFormat.png.ToString())
                || fileType.Equals(ImageFormat.jpeg.ToString())
                || fileType.Equals(ImageFormat.jpg.ToString())
                || fileType.Equals(ImageFormat.bmp.ToString())
                );
        }

      
    }
}
