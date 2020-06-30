using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetService : IDatasetService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public DatasetService(SepesDbContext db, IMapper mapper)
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

        public async Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync()
        {
            var datasetsFromDb = await _db.Datasets
                .Where(ds => ds.StudyID == null)
                .ToListAsync();
            var dataasetsDtos = _mapper.Map<IEnumerable<DatasetListItemDto>>(datasetsFromDb);

            return dataasetsDtos;  
        }

        public async Task<DatasetDto> GetDatasetByIdAsync(int id)
        {
            var datasetFromDb = await GetDatasetOrThrowAsync(id);

            var datasetDto = _mapper.Map<DatasetDto>(datasetFromDb);

            return datasetDto;
        } 
        
        async Task<Dataset> GetDatasetOrThrowAsync(int id)
        {
            var datasetFromDb = await _db.Datasets
                .Where(ds => ds.StudyID == null)
                .Include(s => s. StudyDatasets)
                .ThenInclude(sd=> sd.Study)
                .FirstOrDefaultAsync(s => s.Id == id);

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
