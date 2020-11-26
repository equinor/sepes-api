using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
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

        public async Task<IEnumerable<StudyListItemDto>> GetStudyListAsync(bool? excludeHidden = null)
        {
            List<Study> studiesFromDb;

            if (excludeHidden.HasValue && excludeHidden.Value)
            {
                studiesFromDb = await StudyBaseQueries.UnHiddenStudiesQueryable(_db).ToListAsync();
            }
            else
            {
                var user = await _userService.GetCurrentUserFromDbAsync();
                var studiesQueryable = StudyPluralQueries.ActiveStudiesIncludingHiddenQueryable(_db, user.Id);
                studiesFromDb = await studiesQueryable.ToListAsync();
            }

            var studiesDtos = _mapper.Map<IEnumerable<StudyListItemDto>>(studiesFromDb);


            studiesDtos = _azureBlobStorageService.DecorateLogoUrlsWithSAS(studiesDtos);
            return studiesDtos;
        }

        async Task<Study> GetStudyByIdAsync(int studyId, UserOperation userOperation, bool withIncludes)
        {
            return await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, userOperation, withIncludes);
        }

        public async Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, userOperation, false);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);

            return studyDto;
        }

        public async Task<StudyDetailsDto> GetStudyDetailsDtoByIdAsync(int studyId, UserOperation userOperation)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, userOperation, true);
            var studyDetailsDto = _mapper.Map<StudyDetailsDto>(studyFromDb);
            _azureBlobStorageService.DecorateLogoUrlWithSAS(studyDetailsDto);
            studyDetailsDto.Sandboxes = studyDetailsDto.Sandboxes.Where(sb => !sb.Deleted).ToList();
            await StudyPermissionsUtil.DecorateDto(_userService, studyFromDb, studyDetailsDto.Permissions);


            foreach (var curDs in studyDetailsDto.Datasets)
            {
                curDs.SandboxDatasets = curDs.SandboxDatasets.Where(sd => sd.StudyId == studyId).ToList();
            }

            return studyDetailsDto;
        }


        public async Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudyDto)
        {
            StudyAccessUtil.CheckOperationPermissionsOrThrow(_userService, UserOperation.Study_Create);

            var studyDb = _mapper.Map<Study>(newStudyDto);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();
            MakeCurrentUserOwnerOfStudy(studyDb, currentUser);

            var newStudyId = await Add(studyDb);
            return await GetStudyDtoByIdAsync(newStudyId, UserOperation.Study_Read);
        }

        public async Task<StudyDto> UpdateStudyMetadataAsync(int studyId, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(studyId, updatedStudy);

            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Update_Metadata, false);

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

            return await GetStudyDtoByIdAsync(studyFromDb.Id, UserOperation.Study_Update_Metadata);
        }



        public async Task CloseStudyAsync(int studyId)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Close, true);

            ValidateStudyForCloseOrDeleteThrowIfNot(studyFromDb);

            //await RemoveDatasets(studyFromDb.Id);

            var currentUser = _userService.GetCurrentUser();
            studyFromDb.Closed = true;
            studyFromDb.ClosedBy = currentUser.UserName;
            studyFromDb.ClosedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteStudyAsync(int studyId)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Delete, true);

            ValidateStudyForCloseOrDeleteThrowIfNot(studyFromDb);

            if (!String.IsNullOrWhiteSpace(studyFromDb.LogoUrl))
            {
                _ = _azureBlobStorageService.DeleteBlob(studyFromDb.LogoUrl);
            }

            // TODO: Possibly keep datasets for archiving/logging purposes.

            var studySpecificDatasets = new List<int>();

            //Delete datasets links and study specific datasets
            var studyDatasets = studyFromDb.StudyDatasets.ToList();

            if (studyDatasets.Any())
            {
                foreach (var studyDataset in studyDatasets)
                {
                   //Remove relation
                    studyFromDb.StudyDatasets.Remove(studyDataset);

                    if (studyDataset.Dataset.StudyId == studyFromDb.Id)
                    {
                        //Study specific dataset, must be deleted
                        studySpecificDatasets.Add(studyDataset.DatasetId);
                    } 
                }
            }

            await _db.SaveChangesAsync();

            if (studySpecificDatasets.Any())
            {
                foreach(var curStudySpecificDatasetId in studySpecificDatasets)
                {
                    var datasetToDelete = await _db.Datasets.FirstOrDefaultAsync(d => d.Id == curStudySpecificDatasetId && d.StudyId.HasValue && d.StudyId == studyFromDb.Id);

                    if(datasetToDelete != null)
                    {
                        _db.Datasets.Remove(datasetToDelete);
                    }
                }
            }

            var userEntriesForDeletedStudyParticipants = new HashSet<int>();

            foreach (var curSandbox in studyFromDb.Sandboxes)
            {
                foreach (var curResource in curSandbox.Resources)
                {
                    foreach (var curOperation in curResource.Operations)
                    {
                        if (curOperation.DependsOnOperation != null)
                        {
                            if (_db.SandboxResourceOperations.Contains(curOperation.DependsOnOperation))
                            {
                                _db.SandboxResourceOperations.Remove(curOperation.DependsOnOperation);
                            }
                        }

                        if (_db.SandboxResourceOperations.Contains(curOperation))
                        {
                            _db.SandboxResourceOperations.Remove(curOperation);
                        }
                    }

                    _db.SandboxResources.Remove(curResource);
                }

                _db.Sandboxes.Remove(curSandbox);
            }

            foreach (var curParticipant in studyFromDb.StudyParticipants)
            {
                if (!userEntriesForDeletedStudyParticipants.Contains(curParticipant.UserId))
                {
                    userEntriesForDeletedStudyParticipants.Add(curParticipant.UserId);
                }

                _db.StudyParticipants.Remove(curParticipant);

            }

            await _db.SaveChangesAsync();

            foreach (var curUserId in userEntriesForDeletedStudyParticipants)
            {
                var userEntry = await _db.Users.Include(u => u.StudyParticipants).FirstOrDefaultAsync(u => u.Id == curUserId);

                if(userEntry != null)
                {
                    if(userEntry.StudyParticipants.Count == 0)
                    {
                        _db.Users.Remove(userEntry);
                        await _db.SaveChangesAsync();
                    }
                }              
            }
        }

        void ValidateStudyForCloseOrDeleteThrowIfNot(Study studyFromDb)
        {
            foreach (var curSandbox in studyFromDb.Sandboxes)
            {
                if (curSandbox.Deleted.HasValue == false || curSandbox.DeletedAt.HasValue == false)
                {
                    throw new Exception($"Cannot delete study {studyFromDb.Id}, it has open sandboxes that must be deleted first");
                }
            }
        }       

        async Task RemoveDatasets(int studyId)
        {
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
        }

        public async Task<StudyDto> AddLogoAsync(int studyId, IFormFile studyLogo)
        {
            var fileName = _azureBlobStorageService.UploadBlob(studyLogo);
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Update_Metadata, false);

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

            return await GetStudyDtoByIdAsync(studyFromDb.Id, UserOperation.Study_Update_Metadata);
        }

        public async Task<LogoResponseDto> GetLogoAsync(int studyId)
        {
            try
            {
                var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Read, false);
                var response = new LogoResponseDto() { LogoUrl = studyFromDb.LogoUrl, LogoBytes = await _azureBlobStorageService.GetImageFromBlobAsync(studyFromDb.LogoUrl) };

                return response;
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
