using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ParticipantService : IParticipantService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public ParticipantService(SepesDbContext db, IMapper mapper)
        {            
            _db = db;
            _mapper = mapper;
        }

        //public Task<StudyDto> CreateStudyAsync(StudyDto newStudy)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public Task<StudyDto> DeleteStudyAsync(StudyDto newStudy)
        //{
        //    throw new System.NotImplementedException();
        //}

        public async Task<IEnumerable<ParticipantListItemDto>> GetLookupAsync()
        {
            var datasetsFromDb = await _db.Part.ToListAsync();
            var dataasetsDtos = _mapper.Map<IEnumerable<DatasetListItemDto>>(datasetsFromDb);

            return dataasetsDtos;  
        }

        public async Task<ParticipantDto> GetByIdAsync(int id)
        {
            var datasetFromDb = await GetDatasetOrThrowAsync(id);

            var datasetDto = _mapper.Map<DatasetDto>(datasetFromDb);

            return datasetDto;
        } 
        
        async Task<Dataset> GetParticipantOrThrowAsync(int id)
        {
            var datasetFromDb = await _db.Datasets.Include(s => s. StudyDatasets).ThenInclude(sd=> sd.Study).FirstOrDefaultAsync(s => s.Id == id);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", id);
            }

            return datasetFromDb;
        }

        //public Task<StudyDto> UpdateDatasetAsync(int id, DatasetDto datasetToUpdate)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
