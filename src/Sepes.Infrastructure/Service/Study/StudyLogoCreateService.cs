using Microsoft.AspNetCore.Http;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyLogoCreateService : IStudyLogoCreateService
    {
        readonly string _containerName = "studylogos";
       
        readonly SepesDbContext _db;
        readonly IStudyEfModelService _studyModelService;
        readonly IAzureBlobStorageService _azureBlobStorageService;
        readonly IAzureBlobStorageUriBuilderService _azureStorageAccountTokenService;

        public StudyLogoCreateService(SepesDbContext db,
            IStudyEfModelService studyModelService,
            IAzureBlobStorageService blobService,
            IAzureBlobStorageUriBuilderService azureStorageAccountTokenService)
        {           
            _db = db;        
            _studyModelService = studyModelService;
            _azureBlobStorageService = blobService;
            _azureStorageAccountTokenService = azureStorageAccountTokenService;

            _azureBlobStorageService.SetConnectionParameters(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
            _azureStorageAccountTokenService.SetConnectionParameters(ConfigConstants.STUDY_LOGO_STORAGE_CONSTRING);
        }       

        public async Task<string> CreateAsync(int studyId, IFormFile studyLogo)
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
