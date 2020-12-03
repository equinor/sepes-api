using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
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
        readonly IStudyLogoService _studyLogoService;

        public StudyService(SepesDbContext db, IMapper mapper, ILogger<StudyService> logger, IUserService userService, IStudyLogoService studyLogoService)
            : base(db, mapper)
        {
            _logger = logger;
            _userService = userService;
            _studyLogoService = studyLogoService;
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


            studiesDtos = _studyLogoService.DecorateLogoUrlsWithSAS(studiesDtos);
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
            _studyLogoService.DecorateLogoUrlWithSAS(studyDetailsDto);
            studyDetailsDto.Sandboxes = studyDetailsDto.Sandboxes.Where(sb => !sb.Deleted).ToList();
            await StudyPermissionsUtil.DecorateDto(_userService, studyFromDb, studyDetailsDto.Permissions);


            foreach (var curDs in studyDetailsDto.Datasets)
            {
                curDs.SandboxDatasets = curDs.SandboxDatasets.Where(sd => sd.StudyId == studyId).ToList();
            }

            return studyDetailsDto;
        }


        public async Task<StudyDetailsDto> CreateStudyAsync(StudyCreateDto newStudyDto)
        {
            StudyAccessUtil.HasAccessToOperationOrThrow(_userService, UserOperation.Study_Create);

            var studyDb = _mapper.Map<Study>(newStudyDto);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();
            MakeCurrentUserOwnerOfStudy(studyDb, currentUser);

            var newStudyId = await Add(studyDb);
            return await GetStudyDetailsDtoByIdAsync(newStudyId, UserOperation.Study_Create);
        }

        public async Task<StudyDetailsDto> UpdateStudyMetadataAsync(int studyId, StudyDto updatedStudy)
        {
            var studyFromDb = await GetStudyByIdAsync(studyId, UserOperation.Study_Update_Metadata, false);

            PerformUsualTestsForPostedStudy(studyId, updatedStudy);

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

            return await GetStudyDetailsDtoByIdAsync(studyFromDb.Id, UserOperation.Study_Update_Metadata);
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

            await _studyLogoService.DeleteAsync(studyFromDb);

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
