using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class StudyService : ServiceBase<Study>, IStudyService
    {
        readonly IUserService _userService;
        readonly IAzureBlobStorageService _azureBlobStorageService;
        readonly IAzureADUsersService _azureADUsersService;

        public StudyService(IUserService userService, SepesDbContext db, IMapper mapper, IAzureBlobStorageService azureBlobStorageService, IAzureADUsersService azureADUsersService)
            : base(db, mapper)
        {
            this._userService = userService;
            _azureBlobStorageService = azureBlobStorageService;
            _azureADUsersService = azureADUsersService;
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? includeRestricted = null)
        {
            List<Study> studiesFromDb;

            if (includeRestricted.HasValue && includeRestricted.Value)
            {
                var user = await _userService.GetCurrentUserFromDbAsync();

                var studiesQueryable = GetStudiesIncludingRestrictedForCurrentUser(_db, user.Id);
                studiesFromDb = await studiesQueryable.ToListAsync();
            }
            else
            {
                studiesFromDb = await _db.Studies.Where(s => !s.Restricted).ToListAsync();
            }

            var studiesDtos = _mapper.Map<IEnumerable<StudyListItemDto>>(studiesFromDb);

            studiesDtos = await _azureBlobStorageService.DecorateLogoUrlsWithSAS(studiesDtos);
            return studiesDtos;
        }

        async Task ThrowIfUserDoesNotHaveAccess(Study study)
        {
            if (study.Restricted)
            {
                var currentUser = await _userService.GetCurrentUserFromDbAsync();

                if (study.StudyParticipants.Where(sp => sp.UserId == currentUser.Id).Any() == false)
                {
                    //User does has study specific roles
                    throw new ForbiddenException($"User {currentUser.UserName} does not have access to study {study.Id}");
                }
            }
        }

        public async Task<StudyDto> GetStudyByIdAsync(int studyId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            await ThrowIfUserDoesNotHaveAccess(studyFromDb);

            var studyDto = _mapper.Map<StudyDto>(studyFromDb);
            studyDto.Sandboxes = studyDto.Sandboxes.Where(sb => !sb.Deleted).ToList();

            studyDto = await _azureBlobStorageService.DecorateLogoUrlWithSAS(studyDto);

            return studyDto;
        }

        public async Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudyDto)
        {
            var studyDb = _mapper.Map<Study>(newStudyDto);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();
            AddCurrentUserAsParticipant(studyDb, currentUser);


            var newStudyId = await Add(studyDb);
            return await GetStudyByIdAsync(newStudyId);
        }



        public async Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(studyId, updatedStudy);

            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            await ThrowIfUserDoesNotHaveAccess(studyFromDb);

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


        // TODO: Deletion may be changed later to keep database entry, but remove from listing.
        public async Task<IEnumerable<StudyListItemDto>> DeleteStudyAsync(int studyId)
        {
            //TODO: VALIDATION
            //Delete logo from Azure Blob Storage before deleting study.
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            if (!_userService.CurrentUserIsAdmin())
            {
                throw new ForbiddenException("This action requires Admin role!");
            }

            string logoUrl = studyFromDb.LogoUrl;
            if (!String.IsNullOrWhiteSpace(logoUrl))
            {
                _ = _azureBlobStorageService.DeleteBlob(logoUrl);
            }

            //Check if study contains studySpecific Datasets
            List<Dataset> studySpecificDatasets = await _db.Datasets.Where(ds => ds.StudyId == studyId).ToListAsync();
            if (studySpecificDatasets.Any())
            {
                foreach (Dataset dataset in studySpecificDatasets)
                {
                    // TODO: Possibly keep datasets for archiving/logging purposes.
                    // Possibly: Datasets.removeWithoutDeleting(dataset)
                    _db.Datasets.Remove(dataset);
                }
            }

            //Delete study
            // TODO: Possibly keep study for archiving/logging purposes.
            // Possibly: Studies.removeWithoutDeleting(study) Mark as deleted but keep record?
            _db.Studies.Remove(studyFromDb);
            await _db.SaveChangesAsync();
            return await GetStudiesAsync();
        }

        public async Task<StudyDto> AddLogoAsync(int studyId, IFormFile studyLogo)
        {
            var fileName = _azureBlobStorageService.UploadBlob(studyLogo);
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
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
            var study = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            string logoUrl = study.LogoUrl;
            var logo = _azureBlobStorageService.GetImageFromBlobAsync(logoUrl);
            return await logo;
        }

        public async Task<StudyDto> AddParticipantToStudyAsync(int studyId, int participantId, string role)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var participantFromDb = await _db.Users.FirstOrDefaultAsync(p => p.Id == participantId);

            if (participantFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Participant", participantId);
            }

            //Check that association does not allready exist

            //await VerifyRoleOrThrowAsync(role);

            var studyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, UserId = participantId, RoleName = role };
            await _db.StudyParticipants.AddAsync(studyParticipant);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(studyId);
        }

        public async Task<StudyDto> AddParticipantFromAzureToStudyAsync(int studyId, string participantId, string role)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var participantFromAzure = await _azureADUsersService.GetUser(participantId);

            if (participantFromAzure == null)
            {
                throw new NotFoundException($"Participant with id {participantId} not found!");
            }

            var userDb = new User { EmailAddress = participantFromAzure.Mail, FullName = participantFromAzure.DisplayName, ObjectId = participantId };

            userDb.StudyParticipants = new List<StudyParticipant> { new StudyParticipant { StudyId = studyFromDb.Id, RoleName = role } };


            _db.Users.Add(userDb);
            await _db.SaveChangesAsync();
            return await GetStudyByIdAsync(studyId);

 
        }

        public async Task<StudyDto> HandleAddParticipant(int studyId, AzureADUserDto user, string role)
        {
            if (user.Source == ParticipantSource.Db)
            {
                return await AddParticipantToStudyAsync(studyId, user.DatabaseId , role);
            }
            else if (user.Source == ParticipantSource.Azure)
            {
                return await AddParticipantFromAzureToStudyAsync(studyId, user.Id, role);
            }
            else
            {
                //ToDo Not in azure or in DB
                
            }

            return await GetStudyByIdAsync(studyId);
        }

        public async Task<StudyDto> RemoveParticipantFromStudyAsync(int studyId, int participantId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var participantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.UserId == participantId);

            if (participantFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Participant", participantId);
            }

            studyFromDb.StudyParticipants.Remove(participantFromDb);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(studyId);
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

        void AddCurrentUserAsParticipant(Study study, UserDto user)
        {
            study.StudyParticipants = new List<StudyParticipant>();
            study.StudyParticipants.Add(new StudyParticipant() { UserId = user.Id, RoleName = StudyRoles.StudyOwner, Created = DateTime.UtcNow, CreatedBy = user.UserName });
        }

        IQueryable<Study> GetStudiesIncludingRestrictedForCurrentUser(SepesDbContext db, int userId)
        {
            return db.Studies
                .Include(s=> s.StudyParticipants)
                    .ThenInclude(sp=> sp.User)
                .Where(s => s.Restricted == false || s.StudyParticipants.Where(sp => sp.UserId == userId).Any());
        }

        public async Task<StudyDto> AddNewParticipantToStudyAsync(int studyId, UserCreateDto user)
        {
            if(!checkIfRoleExists(user.Role))
            {
                throw new ArgumentException("Role " + user.Role + " does not exist");
            }
            if(String.IsNullOrWhiteSpace(user.FullName))
            {
                throw new ArgumentException("Name is empty");
            }
            if (String.IsNullOrWhiteSpace(user.EmailAddress))
            {
                throw new ArgumentException("Email is empty");
            }
            if (String.IsNullOrWhiteSpace(user.Role))
            {
                throw new ArgumentException("Role is empty");
            }

            var userDb = _mapper.Map<User>(user);

            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            userDb.StudyParticipants = new List<StudyParticipant> { new StudyParticipant { StudyId = studyFromDb.Id, RoleName = user.Role } };

            var test = _db.StudyParticipants.ToList();
            _db.Users.Add(userDb);
            await _db.SaveChangesAsync();

            //Check that association does not allready exist

            //await VerifyRoleOrThrowAsync(role);

            return await GetStudyByIdAsync(studyId);
        }

        private bool checkIfRoleExists (string Role)
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
