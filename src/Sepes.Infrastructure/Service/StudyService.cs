using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class StudyService : ServiceBase<Study>, IStudyService
    {
        readonly IAzureBlobStorageService _azureBlobStorageService;

        public StudyService(SepesDbContext db, IMapper mapper, IAzureBlobStorageService azureBlobStorageService)
            :base(db, mapper)
        {
            _azureBlobStorageService = azureBlobStorageService;
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

            studiesDtos = await _azureBlobStorageService.DecorateLogoUrlsWithSAS(studiesDtos);
            return studiesDtos;
        }   

        public async Task<StudyDto> GetStudyByIdAsync(int studyId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);

            studyDto = await _azureBlobStorageService.DecorateLogoUrlWithSAS(studyDto);

            return studyDto;
        }

        public async Task<StudyDto> CreateStudyAsync(StudyDto newStudy)
        {
            var newStudyDbModel = _mapper.Map<Study>(newStudy);

            var newStudyId = await Add(newStudyDbModel);       

            return await GetStudyByIdAsync(newStudyId);
        }

        public async Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto updatedStudy)
        {
            PerformUsualTestsForPostedStudy(studyId, updatedStudy);

            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

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

        // TODO: Deletion may be changed later to keep database entry, but remove from listing.
        public async Task<IEnumerable<StudyListItemDto>> DeleteStudyAsync(int studyId)
        {
            //TODO: VALIDATION
            //Delete logo from Azure Blob Storage before deleting study.
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            string logoUrl = studyFromDb.LogoUrl;
            if (!String.IsNullOrWhiteSpace(logoUrl))
            {            
                _ = _azureBlobStorageService.DeleteBlob(logoUrl);
            }

            //Check if study contains studySpecific Datasets
            List<Dataset> studySpecificDatasets = await _db.Datasets.Where(ds => ds.StudyNo == studyId).ToListAsync();
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
    }
}
