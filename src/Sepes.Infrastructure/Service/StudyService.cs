using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
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


        public async Task<StudyDto> GetStudyByIdAsync(int studyId)
        {
            //var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyReadOwnRestricted);

            var studyDto = _mapper.Map<StudyDto>(studyFromDb);
            studyDto.Sandboxes = studyDto.Sandboxes.Where(sb => !sb.Deleted).ToList();

            studyDto = await _azureBlobStorageService.DecorateLogoUrlWithSAS(studyDto);

            return studyDto;
        }

        public async Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudyDto)
        {
            var studyDb = _mapper.Map<Study>(newStudyDto);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();
            MakeCurrentUserOwnerOfStudy(studyDb, currentUser);

            var newStudyId = await Add(studyDb);
            return await GetStudyByIdAsync(newStudyId);
        }



        public async Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(studyId, updatedStudy);

            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyUpdateMetadata);

            if (!String.IsNullOrWhiteSpace(updatedStudy.Name) && updatedStudy.Name != studyFromDb.Name)
            {
                studyFromDb.Name = updatedStudy.Name;
            }

            if (updatedStudy.Description != studyFromDb.Description)
            {
                studyFromDb.Description = updatedStudy.Description;
            }

            if (!String.IsNullOrWhiteSpace(updatedStudy.Vendor) && updatedStudy.Vendor != studyFromDb.Vendor)
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

            return await GetStudyByIdAsync(studyFromDb.Id);
        }
      
        public async Task<StudyDto> DeleteStudyAsync(int studyId)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyDelete);

            foreach(var curSandbox in studyFromDb.Sandboxes)
            {
                if(curSandbox.DeletedAt.HasValue == false)
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
            studyFromDb.DeletedBy = currentUser.UserName;
            studyFromDb.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return _mapper.Map<StudyDto>(studyFromDb);
        }

        public async Task<StudyDto> AddLogoAsync(int studyId, IFormFile studyLogo)
        {
            var fileName = _azureBlobStorageService.UploadBlob(studyLogo);
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyUpdateMetadata);

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

            return await GetStudyByIdAsync(studyFromDb.Id);
        }

        public async Task<byte[]> GetLogoAsync(int studyId)
        {
            try
            {
                var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyReadOwnRestricted);
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

        //public async Task<StudyDto> AddNewParticipantToStudyAsync(int studyId, UserCreateDto user)
        //{
        //    if(!checkIfRoleExists(user.Role))
        //    {
        //        throw new ArgumentException("Role " + user.Role + " does not exist");
        //    }
        //    if(String.IsNullOrWhiteSpace(user.FullName))
        //    {
        //        throw new ArgumentException("Name is empty");
        //    }
        //    if (String.IsNullOrWhiteSpace(user.EmailAddress))
        //    {
        //        throw new ArgumentException("Email is empty");
        //    }
        //    if (String.IsNullOrWhiteSpace(user.Role))
        //    {
        //        throw new ArgumentException("Role is empty");
        //    }

        //    var userDb = _mapper.Map<User>(user);

        //    var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

        //    userDb.StudyParticipants = new List<StudyParticipant> { new StudyParticipant { StudyId = studyFromDb.Id, RoleName = user.Role } };

        //    var test = _db.StudyParticipants.ToList();
        //    _db.Users.Add(userDb);
        //    await _db.SaveChangesAsync();

        //    //Check that association does not allready exist

        //    //await VerifyRoleOrThrowAsync(role);

        //    return await GetStudyByIdAsync(studyId);
        //}

        private bool checkIfRoleExists(string Role)
        {
            if (Role.Equals(StudyRoles.SponsorRep) ||
                Role.Equals(StudyRoles.StudyOwner) ||
                Role.Equals(StudyRoles.StudyViewer) ||
                Role.Equals(StudyRoles.VendorAdmin) ||
                Role.Equals(StudyRoles.VendorContributor))
            {
                return true;
            }
            return false;
        }
    }
}
