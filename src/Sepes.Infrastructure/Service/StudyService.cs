using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
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

        public Task<StudyDto> CreateStudyAsync(StudyDto newStudy)
        {
            throw new System.NotImplementedException();
        }

        public Task<StudyDto> DeleteStudyAsync(StudyDto newStudy)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<StudyDto>> GetStudiesAsync()
        {
            var studiesFromDb = await _db.Studies.ToListAsync();

            var studiesDtos = _mapper.Map<IEnumerable<StudyDto>>(studiesFromDb);

            return studiesDtos;  
        }

        public async Task<StudyDto> GetStudyByIdAsync(int id)
        {
            var studyFromDb = await _db.Studies.FirstOrDefaultAsync(s=> s.Id == id);

            var studyDto = _mapper.Map<StudyDto>(studyFromDb);

            return studyDto;
        }       

        public Task<StudyDto> UpdateStudyAsync(int id, StudyDto newStudy)
        {
            throw new System.NotImplementedException();
        }
    }
}
