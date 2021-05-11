using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyLogoService : IStudyLogoService
    {
        readonly string _containerName = "studylogos";

        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IStudyModelService _studyModelService;
        readonly IAzureBlobStorageService _azureBlobStorageService;
        readonly IAzureBlobStorageUriBuilderService _azureStorageAccountTokenService;

        public StudyLogoService(ILogger<StudyLogoService> logger, SepesDbContext db,
            IStudyModelService studyModelService,
            IAzureBlobStorageService blobService,
            IAzureBlobStorageUriBuilderService azureStorageAccountTokenService)
        {
            _logger = logger;
            _db = db;        
            _studyModelService = studyModelService;
            _azureBlobStorageService = blobService;
            _azureStorageAccountTokenService = azureStorageAccountTokenService;

            _azureBlobStorageService.SetConnectionParameters(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
            _azureStorageAccountTokenService.SetConnectionParameters(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
        }

        public async Task DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo)
        {
            try
            {
                var uriBuilder = await CreateFileDownloadUriBuilder(_containerName);

                if (uriBuilder == null)
                {
                    _logger.LogError("Unable to decorate logo urls");
                }
                else
                {
                    DecorateLogoUrlWithSAS(uriBuilder, hasLogo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to decorate Study item with logo url. Se exception info");
            }
        }

        public async Task DecorateLogoUrlsWithSAS(IEnumerable<IHasLogoUrl> studyDtos)
        {
            try
            {
                var uriBuilder = await CreateFileDownloadUriBuilder(_containerName);

                if (uriBuilder == null)
                {
                    _logger.LogError("Unable to decorate logo urls");
                }
                else
                {
                    foreach (var curDto in studyDtos)
                    {
                        DecorateLogoUrlWithSAS(uriBuilder, curDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to decorate list of Studies with logo urls. Se exception info");
            }
        }

        async Task<UriBuilder> CreateFileDownloadUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await _azureStorageAccountTokenService.CreateUriBuilder(containerName, cancellationToken: cancellationToken);
        }

        void DecorateLogoUrlWithSAS(UriBuilder uriBuilder, IHasLogoUrl hasLogo)
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

        public async Task<string> AddLogoAsync(int studyId, IFormFile studyLogo)
        {
            var studyFromDb = await _studyModelService.GetByIdAsync(studyId, UserOperation.Study_Update_Metadata);

            if (!FileIsCorrectImageFormat(studyLogo))
            {
                throw new ArgumentException("Blob has invalid filename or is not of type png, jpg or bmp.");
            }

            string uniqueFileName = Guid.NewGuid().ToString("N") + studyLogo.FileName;

            await _azureBlobStorageService.UploadFileToBlobContainer(_containerName, uniqueFileName, studyLogo);

            string oldFileName = studyFromDb.LogoUrl;

            studyFromDb.LogoUrl = uniqueFileName;

            await _db.SaveChangesAsync();

            if (!String.IsNullOrWhiteSpace(oldFileName))
            {
                _ = await _azureBlobStorageService.DeleteFileFromBlobContainer(_containerName, oldFileName);
            }

            return studyFromDb.LogoUrl;
        }

        public async Task DeleteAsync(Study study)
        {
            if (!String.IsNullOrWhiteSpace(study.LogoUrl))
            {
                _ = await _azureBlobStorageService.DeleteFileFromBlobContainer(_containerName, study.LogoUrl);
            }
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
