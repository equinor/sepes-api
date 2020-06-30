using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;


namespace Sepes.Infrastructure.Service
{
    public class StudyService : ServiceBase<Study>, IStudyService
    {
        private readonly IConfiguration _configuration;
        public StudyService(SepesDbContext db, IMapper mapper, IConfiguration configuration)
            :base(db, mapper)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? includeRestricted = null)
        {
            List<Study> studiesFromDb;
            if (includeRestricted.HasValue && includeRestricted.Value)
            {
                // TODO: Add authorization
                //if (!(await UserCanSeeRestrictedStudies()))
                //{
                //    //TODO: THROW EXCEPTION THAT CAUSES 401
                //}
                //else
                //{
                //    // Get restricted studies 
                //}
                studiesFromDb = await _db.Studies.ToListAsync();
            }
            else
            {
                studiesFromDb = await _db.Studies.Where(s => !s.Restricted).ToListAsync();
            }

            var studiesDtos = _mapper.Map<IEnumerable<StudyListItemDto>>(studiesFromDb);
            return studiesDtos;
        }            

        public async Task<StudyDto> GetStudyByIdAsync(int id)
        {
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);

            return studyDto;
        }

        public async Task<StudyDto> CreateStudyAsync(StudyDto newStudy)
        {
            var newStudyDbModel = _mapper.Map<Study>(newStudy);

            var newStudyId = await Add(newStudyDbModel);       

            return await GetStudyByIdAsync(newStudyId);
        }

        public async Task<StudyDto> UpdateStudyDetailsAsync(int id, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(id, updatedStudy);

            var studyFromDb = await GetStudyOrThrowAsync(id);

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

            Validate(studyFromDb);

            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(studyFromDb.Id);
        }

        void PerformUsualTestsForPostedStudy(int id, StudyDto updatedStudy)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Id was zero or negative:" + id);
            }

            if (id != updatedStudy.Id)
            {
                throw new ArgumentException($"Id in url ({id}) is different from Id in data ({updatedStudy.Id})");
            }
        }

        public async Task<IEnumerable<StudyListItemDto>> DeleteStudyAsync(int id)
        {
            //TODO: VALIDATION
            //Delete logo from Azure Blob Storage before deleting study.
            var studyFromDb = await GetStudyOrThrowAsync(id);
            string logoUrl = studyFromDb.LogoUrl;
            if (!String.IsNullOrWhiteSpace(logoUrl))
            {
                string storageConnectionString = _configuration["ConnectionStrings:AzureStorageConnectionString"];
                var blobStorage = new AzureBlobStorageService(storageConnectionString);
                _ = blobStorage.DeleteBlob(logoUrl);
            }
            //Delete study
            _db.Studies.Remove(studyFromDb);
            await _db.SaveChangesAsync();
            return await GetStudiesAsync();
        }

        async Task<Study> GetStudyOrThrowAsync(int id)
        {
            var studyFromDb = await _db.Studies
                .Include(s => s.StudyDatasets)
                .ThenInclude(sd => sd.Dataset)
                .Include(s => s.Sandboxes)
                .Include(s => s.StudyParticipants)
                     .ThenInclude(sp => sp.Participant)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Study", id);
            }

            return studyFromDb;
        }

        public async Task<StudyDto> AddDatasetAsync(int id, int datasetId)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if(datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", datasetId);
            }

            if(datasetFromDb.StudyID != null)
            {
                throw new ArgumentException($"Dataset with id {datasetId} is studySpecific, and cannot be linked using this method.");
            }

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset{ Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task<StudyDto> AddStudySpecificDatasetAsync(int id, StudySpecificDatasetDto newDataset)
        {
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var dataset = _mapper.Map<Dataset>(newDataset);
            dataset.StudyID = id;
            await _db.Datasets.AddAsync(dataset);
            await _db.SaveChangesAsync();

            var datasetFromDb = await _db.Datasets.Where(ds => ds.StudyID == id).FirstOrDefaultAsync();
            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", id);
            }

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset { Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task<StudyDto> RemoveDatasetAsync(int id, int datasetId)
        {
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            //Does dataset exist?
            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", datasetId);
            }

            var studyDatasetFromDb = await _db.StudyDatasets
                .FirstOrDefaultAsync(ds => ds.StudyId == id && ds.DatasetId == datasetId);

            //Is dataset linked to a study?
            if (studyDatasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("StudyDataset", datasetId);
            }

            _db.StudyDatasets.Remove(studyDatasetFromDb);

            //If dataset is studyspecific, remove dataset as well.
            if (datasetFromDb.StudyID != null)
            {
                _db.Datasets.Remove(datasetFromDb);
            }

            await _db.SaveChangesAsync();
            var retVal = await GetStudyByIdAsync(id);
            return retVal;
        }

        public async Task<StudyDto> AddSandboxAsync(int id, SandboxDto newSandbox)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await GetStudyOrThrowAsync(id);

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(studyFromDb.WbsCode))
            {
                throw new ArgumentException("WBS code missing in Study. Study requires WBS code before sandbox can be created.");
            }
            // TODO: Do check on Sandbox

            // Create reference
            var sandbox = _mapper.Map<Sandbox>(newSandbox);
            studyFromDb.Sandboxes.Add(sandbox);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task<StudyDto> RemoveSandboxAsync(int id, int sandboxId)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var sandboxFromDb = await _db.Sandboxes.FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            // TODO: Do check on Sandbox

            // Create reference
            //var sandbox = _mapper.Map<Sandbox>(sandboxFromDb);
            studyFromDb.Sandboxes.Remove(sandboxFromDb);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task<IEnumerable<SandboxDto>> GetSandboxesByStudyIdAsync(int id)
        {
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == id).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        public async Task<StudyDto> AddParticipantAsync(int id, int participantId, string role)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var participantFromDb = await _db.Participants.FirstOrDefaultAsync(p => p.Id == participantId);

            if (participantFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Participant", participantId);
            }

            //Check that association does not allready exist

            await VerifyRoleOrThrowAsync(role);

            var studyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, ParticipantId = participantId, RoleName = role };
            await _db.StudyParticipants.AddAsync(studyParticipant);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task VerifyRoleOrThrowAsync(string roleName)
        {
            var roleExists = false;

            var roleIsPermittedForParticipant = false;

        }

        public async Task<StudyDto> RemoveParticipantAsync(int id, int participantId)
        {          
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var participantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.ParticipantId == participantId);

            if (participantFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Participant", participantId);
            }
          
            studyFromDb.StudyParticipants.Remove(participantFromDb);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo)
        {
            string storageConnectionString = _configuration["ConnectionStrings:AzureStorageConnectionString"];
            var blobStorage = new AzureBlobStorageService(storageConnectionString);
            var fileName = blobStorage.UploadBlob(studyLogo);
            var studyFromDb = await GetStudyOrThrowAsync(id);
            string oldFileName = studyFromDb.LogoUrl;

            if (!String.IsNullOrWhiteSpace(fileName) && oldFileName != fileName)
            {
                studyFromDb.LogoUrl = fileName;
            }

            Validate(studyFromDb);
            await _db.SaveChangesAsync();

            if (!String.IsNullOrWhiteSpace(oldFileName))
            {
            _ = blobStorage.DeleteBlob(oldFileName);
            }

            return await GetStudyByIdAsync(studyFromDb.Id);
        }

        public async Task<byte[]> GetLogoAsync(int id)
        {
            string storageConnectionString = _configuration["ConnectionStrings:AzureStorageConnectionString"];
            var blobStorage = new AzureBlobStorageService(storageConnectionString);
            var study = await GetStudyOrThrowAsync(id);
            string logoUrl = study.LogoUrl;
            var logo = blobStorage.GetImageFromBlobAsync(logoUrl);
            return await logo;
        }
    }
}
