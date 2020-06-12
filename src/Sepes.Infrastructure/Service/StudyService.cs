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

        public async Task<IEnumerable<StudyListItemDto>> GetStudiesAsync()
        {
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
            if(newStudy.Id.HasValue)
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

            //If no errors, return true, false otherwise
            return String.IsNullOrWhiteSpace(validationErrors);
        }

        public Task<StudyDto> UpdateStudyAsync(int id, StudyDto newStudy)
        {
            throw new System.NotImplementedException();
        }

        public Task<StudyDto> DeleteStudyAsync(StudyDto newStudy)
        {
            throw new System.NotImplementedException();
        }      
        
        async Task<Study> GetStudyOrThrowAsync(int id)
        {
            var studyFromDb = await _db.Studies
                .Include(s => s.StudyDatasets)
                .ThenInclude(sd=> sd.Dataset)
                .Include(s => s.SandBoxes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Study", id);
            }

            return studyFromDb;
        }

      
    }
}
