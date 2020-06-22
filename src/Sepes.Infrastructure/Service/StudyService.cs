using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyService : IStudyService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public StudyService(SepesDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? includeRestricted = null)
        {
            if (includeRestricted.HasValue && includeRestricted.Value)
            {
                if (!(await UserCanSeeRestrictedStudies()))
                {
                    //TODO: THROW EXCEPTION THAT CAUSES 401
                }
                else
                {
                    // Get restricted studies 
                }
            }

            var studiesFromDb = await _db.Studies.ToListAsync();
            var studiesDtos = _mapper.Map<IEnumerable<StudyListItemDto>>(studiesFromDb);
            return studiesDtos;
        }

        async Task<bool> UserCanSeeRestrictedStudies()
        {
            //TODO: Implement
            return true;
        }       

        public async Task<StudyDto> GetStudyByIdAsync(int id)
        {
            var studyFromDb = await GetStudyOrThrowAsync(id);
            var studyDto = _mapper.Map<StudyDto>(studyFromDb);

            return studyDto;
        }

        public async Task<StudyDto> CreateStudyAsync(StudyDto newStudy)
        {
            if (newStudy.Id.HasValue)
            {
                throw new ArgumentException("Id for new Study cannot be specified, it's randomly generated. You tried to specify id:" + newStudy.Id.Value);
            }

            if (!ValidateStudy(newStudy, out string validationError))
            {
                throw new ArgumentException(validationError);
            }

            var newStudyDbModel = _mapper.Map<Study>(newStudy);

            _db.Studies.Add(newStudyDbModel);
            await _db.SaveChangesAsync();

            return await GetStudyByIdAsync(newStudyDbModel.Id);
        }

        bool ValidateStudy(StudyDto studyDto, out string validationErrors)
        {
            validationErrors = "";

            if (String.IsNullOrWhiteSpace(studyDto.Name))
            {
                validationErrors += "name must have value";
                return false;
            }

            if (String.IsNullOrWhiteSpace(studyDto.Vendor))
            {
                validationErrors += "vendor must have value";
                return false;
            }

            //If no errors, return true, false otherwise
            return String.IsNullOrWhiteSpace(validationErrors);
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

        public async Task DeleteStudyAsync(int id)
        {
            //TODO: VALIDATION
            var studyFromDb = await GetStudyOrThrowAsync(id);
            _db.Studies.Remove(studyFromDb);
            await _db.SaveChangesAsync();
        }

        async Task<Study> GetStudyOrThrowAsync(int id)
        {
            var studyFromDb = await _db.Studies
                .Include(s => s.StudyDatasets)
                .ThenInclude(sd => sd.Dataset)
                .Include(s => s.SandBoxes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Study", id);
            }

            return studyFromDb;
        }

        public async Task<StudyDto> AddDataset(int id, int datasetId)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        public async Task<StudyDto> AddCustomDataset(int id, int datasetId, StudySpecificDatasetDto newDataset)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }
}
