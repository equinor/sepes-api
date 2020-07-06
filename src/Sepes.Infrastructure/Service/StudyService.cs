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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(id, _db);
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

            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(id, _db);

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

        // TODO: Deletion may be changed later to keep database entry, but remove from listing.
        public async Task<IEnumerable<StudyListItemDto>> DeleteStudyAsync(int id)
        {
            //TODO: VALIDATION
            //Delete logo from Azure Blob Storage before deleting study.
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(id, _db);
            string logoUrl = studyFromDb.LogoUrl;
            if (!String.IsNullOrWhiteSpace(logoUrl))
            {
                string storageConnectionString = _configuration["AzureStorageConnectionString"];
                var blobStorage = new AzureBlobStorageService(storageConnectionString);
                _ = blobStorage.DeleteBlob(logoUrl);
            }

            //Check if study contains studySpecific Datasets
            List<Dataset> studySpecificDatasets = await _db.Datasets.Where(ds => ds.StudyNo == id).ToListAsync();
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

        public async Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo)
        {
            string storageConnectionString = _configuration["AzureStorageConnectionString"];
            var blobStorage = new AzureBlobStorageService(storageConnectionString);
            var fileName = blobStorage.UploadBlob(studyLogo);
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(id, _db);
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
            string storageConnectionString = _configuration["AzureStorageConnectionString"];
            var blobStorage = new AzureBlobStorageService(storageConnectionString);
            var study = await StudyQueries.GetStudyOrThrowAsync(id, _db);
            string logoUrl = study.LogoUrl;
            var logo = blobStorage.GetImageFromBlobAsync(logoUrl);
            return await logo;
        }
    }
}
