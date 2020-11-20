using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class StudyService : ServiceBase<Study>, IStudyService
    {
        readonly ILogger _logger;
        readonly IUserService _userService;
        readonly IAzureBlobStorageService _azureBlobStorageService;

        public StudyService(SepesDbContext db, IMapper mapper, ILogger<StudyService> logger, IUserService userService, IAzureBlobStorageService azureBlobStorageService)
            : base(db, mapper)
        {
            _logger = logger;
            _userService = userService;
            _azureBlobStorageService = azureBlobStorageService;
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? excludeHidden = null)
        {
            List<Study> studiesFromDb;

            if (excludeHidden.HasValue && excludeHidden.Value)
            {
                studiesFromDb = await StudyQueries.UnHiddenStudiesQueryable(_db).ToListAsync();
            }
            else
            {
                var user = await _userService.GetCurrentUserFromDbAsync();
                var studiesQueryable = StudyAccessUtil.GetStudiesIncludingRestrictedForCurrentUser(_db, user.Id);
                studiesFromDb = await studiesQueryable.ToListAsync();
            }

            var studiesDtos = _mapper.Map<IEnumerable<StudyListItemDto>>(studiesFromDb);
          

            studiesDtos = await _azureBlobStorageService.DecorateLogoUrlsWithSAS(studiesDtos);
            return studiesDtos;
        }

        async Task<Study> GetStudyByIdAsync(int studyId, UserOperations userOperation)
        {
            return await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, userOperation);
        }

        public async Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperations userOperation)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, userOperation);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);
            studyDto.Sandboxes = studyDto.Sandboxes.Where(sb => !sb.Deleted).ToList();

            foreach (var curDs in studyDto.Datasets)
            {
                curDs.SandboxDatasets = curDs.SandboxDatasets.Where(sd => sd.StudyId == studyId).ToList();
            }

            studyDto = await _azureBlobStorageService.DecorateLogoUrlWithSAS(studyDto);
            return studyDto;
        }

        public async Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudyDto)
        {
            //TODO: Validate action
            var studyDb = _mapper.Map<Study>(newStudyDto);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();
            MakeCurrentUserOwnerOfStudy(studyDb, currentUser);

            var newStudyId = await Add(studyDb);
            return await GetStudyDtoByIdAsync(newStudyId, UserOperations.StudyRead);
        }

        public async Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(studyId, updatedStudy);

            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperations.StudyUpdateMetadata);

            if (updatedStudy.Name != studyFromDb.Name)
            {
                studyFromDb.Name = updatedStudy.Name;
            }

            if (updatedStudy.Description != studyFromDb.Description)
            {
                studyFromDb.Description = updatedStudy.Description;
            }

            if (updatedStudy.Vendor != studyFromDb.Vendor)
            {
                studyFromDb.Vendor = updatedStudy.Vendor;
            }

            if (updatedStudy.Restricted != studyFromDb.Restricted)
            {
                studyFromDb.Restricted = updatedStudy.Restricted;
            }

            if (updatedStudy.WbsCode != studyFromDb.WbsCode)
            {
                studyFromDb.WbsCode = updatedStudy.WbsCode;
            }

            if (updatedStudy.ResultsAndLearnings != studyFromDb.ResultsAndLearnings)
            {
                studyFromDb.ResultsAndLearnings = updatedStudy.ResultsAndLearnings;
            }

            studyFromDb.Updated = DateTime.UtcNow;

            Validate(studyFromDb);

            await _db.SaveChangesAsync();

            return await GetStudyDtoByIdAsync(studyFromDb.Id, UserOperations.StudyUpdateMetadata);
        }
      
        public async Task DeleteStudyAsync(int studyId)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyDelete);

            foreach(var curSandbox in studyFromDb.Sandboxes)
            {
                if(curSandbox.Deleted.HasValue == false || curSandbox.DeletedAt.HasValue == false)
                {
                    throw new Exception($"Cannot delete study {studyId}, it has open sandboxes that must be deleted first");
                }
            }

            if (!String.IsNullOrWhiteSpace(studyFromDb.LogoUrl))
            {
                _ = _azureBlobStorageService.DeleteBlob(studyFromDb.LogoUrl);
            }

            //Check if study contains studySpecific Datasets
            var studySpecificDatasets = await _db.Datasets.Where(ds => ds.StudyId == studyId).ToListAsync();

            if (studySpecificDatasets.Any())
            {
                foreach (Dataset dataset in studySpecificDatasets)
                {
                    // TODO: Possibly keep datasets for archiving/logging purposes.
                    // Possibly: Datasets.removeWithoutDeleting(dataset)
                    _db.Datasets.Remove(dataset);
                }
            }

            var currentUser = _userService.GetCurrentUser();
            studyFromDb.Deleted = true;
            studyFromDb.DeletedBy = currentUser.UserName;
            studyFromDb.DeletedAt = DateTime.UtcNow;
            
            await _db.SaveChangesAsync();
        }

        public async Task<StudyDto> AddLogoAsync(int studyId, IFormFile studyLogo)
        {
            var fileName = _azureBlobStorageService.UploadBlob(studyLogo);
            var studyFromDb = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyUpdateMetadata);

            string oldFileName = studyFromDb.LogoUrl;

            if (!String.IsNullOrWhiteSpace(fileName) && oldFileName != fileName)
            {
                studyFromDb.LogoUrl = fileName;
            }

            Validate(studyFromDb);
            await _db.SaveChangesAsync();

            if (!String.IsNullOrWhiteSpace(oldFileName))
            {
                _ = _azureBlobStorageService.DeleteBlob(oldFileName);
            }

            return await GetStudyDtoByIdAsync(studyFromDb.Id, UserOperations.StudyUpdateMetadata);
        }

        public async Task<byte[]> GetLogoAsync(int studyId)
        {
            try
            {
                var studyFromDb = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyRead);
                string logoUrl = studyFromDb.LogoUrl;
                var logo = _azureBlobStorageService.GetImageFromBlobAsync(logoUrl);
                return await logo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to get logo for Study {studyId}");
                return null;
            }

        }

        void PerformUsualTestsForPostedStudy(int studyId, StudyDto updatedStudy)
        {
            if (studyId <= 0)
            {
                throw new ArgumentException("Id was zero or negative:" + studyId);
            }

            if (studyId != updatedStudy.Id)
            {
                throw new ArgumentException($"Id in url ({studyId}) is different from Id in data ({updatedStudy.Id})");
            }
        }

        void MakeCurrentUserOwnerOfStudy(Study study, UserDto user)
        {
            study.StudyParticipants = new List<StudyParticipant>();
            study.StudyParticipants.Add(new StudyParticipant() { UserId = user.Id, RoleName = StudyRoles.StudyOwner, Created = DateTime.UtcNow, CreatedBy = user.UserName });
        }
    }
}
