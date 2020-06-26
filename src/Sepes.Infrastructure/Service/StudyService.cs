using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        public StudyService(SepesDbContext db, IMapper mapper)
            :base(db, mapper)
        {
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? includeRestricted = null)
        {
            if (includeRestricted.HasValue && includeRestricted.Value)
            {
                //if (!(await UserCanSeeRestrictedStudies()))
                //{
                //    //TODO: THROW EXCEPTION THAT CAUSES 401
                //}
                //else
                //{
                //    // Get restricted studies 
                //}
            }

            var studiesFromDb = await _db.Studies.ToListAsync();
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

      //public async Task<StudyDto> CreateStudyAsync(StudyDto newStudy, IFormFile studyLogo)
      //{
      //    var newStudyDbModel = _mapper.Map<Study>(newStudy);
      // Not Finished!!
      //    var newStudyId = await Add(newStudyDbModel);
      //
      //    return await GetStudyByIdAsync(newStudyId);
      //}

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

        public async Task<StudyDto> UpdateStudyAsync(int id, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(id, updatedStudy);

            var studyFromDb = await GetStudyOrThrowAsync(id);

            //Validate
            //If okay: save, if not: return message

            await _db.SaveChangesAsync();

            //TODO: Handle update
            //TODO: HANDLE DATA SETS
            //TODO: HANDLE SANDBOXES
            //TODO: HANDLE LOCK/UNLOCK         

            throw new System.NotImplementedException();
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
            var studyFromDb = await GetStudyOrThrowAsync(id);
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

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset{ Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(id);
        }

        public async Task<StudyDto> AddCustomDatasetAsync(int id, int datasetId, StudySpecificDatasetDto newDataset)
        {
            throw new NotImplementedException();
        }

        public async Task<StudyDto> RemoveDatasetAsync(int id, int datasetId)
        {
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", datasetId);
            }

            var studyDatasetFromDb = await _db.StudyDatasets
                .FirstOrDefaultAsync(ds => ds.StudyId == id && ds.DatasetId == datasetId);

            if (studyDatasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("StudyDataset", datasetId);
            }

            _db.StudyDatasets.Remove(studyDatasetFromDb);
            await _db.SaveChangesAsync();
            var retVal = await GetStudyByIdAsync(id);
            return retVal;
        }

        public async Task<StudyDto> AddSandboxAsync(int id, SandboxDto newSandbox)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await GetStudyOrThrowAsync(id);

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

        public async Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo, string connectionString)
        {
            var blobStorage = new AzureBlobStorageService(connectionString);
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

        public async Task<byte[]> GetLogoAsync(int id, string connectionString)
        {
            var blobStorage = new AzureBlobStorageService(connectionString);
            var study = await GetStudyOrThrowAsync(id);
            string logoUrl = study.LogoUrl;
            var logo = blobStorage.GetImageFromBlobAsync(logoUrl);
            return await logo;
        }
    }
}
