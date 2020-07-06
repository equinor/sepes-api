using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class DatasetService : IDatasetService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IStudyService _studyService;

        public DatasetService(SepesDbContext db, IMapper mapper, IStudyService studyService)
        {            
            _db = db;
            _mapper = mapper;
            _studyService = studyService;
        }

        public async Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync()
        {
            var datasetsFromDb = await _db.Datasets
                .Where(ds => ds.StudyNo == null)
                .ToListAsync();
            var dataasetsDtos = _mapper.Map<IEnumerable<DatasetListItemDto>>(datasetsFromDb);

            return dataasetsDtos;  
        }

        public async Task<DatasetDto> GetDatasetByDatasetIdAsync(int id)
        {
            var datasetFromDb = await GetDatasetOrThrowAsync(id);

            var datasetDto = _mapper.Map<DatasetDto>(datasetFromDb);

            return datasetDto;
        }

        public async Task<DatasetDto> GetSpecificDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            var studyDatasetRelation = studyFromDb.StudyDatasets.FirstOrDefault(sd => sd.DatasetId == datasetId);

            if (studyDatasetRelation == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", datasetId);
            }

            var datasetDto = _mapper.Map<DatasetDto>(studyDatasetRelation.Dataset);

            return datasetDto;
        }

        async Task<Dataset> GetDatasetOrThrowAsync(int id)
        {
            var datasetFromDb = await _db.Datasets
                .Where(ds => ds.StudyNo == null)
                .Include(s => s. StudyDatasets)
                .ThenInclude(sd=> sd.Study)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", id);
            }

            return datasetFromDb;
        }

        public async Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", datasetId);
            }

            if (datasetFromDb.StudyNo != null)
            {
                throw new ArgumentException($"Dataset with id {datasetId} is studySpecific, and cannot be linked using this method.");
            }

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset { Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyByIdAsync(studyId);
        }

        public async Task<StudyDto> RemoveDatasetFromStudyAsync(int studyId, int datasetId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            //Does dataset exist?
            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Dataset", datasetId);
            }

            var studyDatasetFromDb = await _db.StudyDatasets
                .FirstOrDefaultAsync(ds => ds.StudyId == studyId && ds.DatasetId == datasetId);

            //Is dataset linked to a study?
            if (studyDatasetFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("StudyDataset", datasetId);
            }

            _db.StudyDatasets.Remove(studyDatasetFromDb);

            //If dataset is studyspecific, remove dataset as well.
            // Possibly keep database entry, but mark as deleted.
            if (datasetFromDb.StudyNo != null)
            {
                _db.Datasets.Remove(datasetFromDb);
            }

            await _db.SaveChangesAsync();
            var retVal = await _studyService.GetStudyByIdAsync(studyId);
            return retVal;
        }

        public async Task<StudyDto> AddStudySpecificDatasetAsync(int studyId, StudySpecificDatasetDto newDataset)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            PerformUsualTestForPostedDatasets(newDataset);
            var dataset = _mapper.Map<Dataset>(newDataset);
            dataset.StudyNo = studyId;
            await _db.Datasets.AddAsync(dataset);

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset { Study = studyFromDb, Dataset = dataset };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyByIdAsync(studyId);
        }

        void PerformUsualTestForPostedDatasets(StudySpecificDatasetDto datasetDto)
        {
            if (String.IsNullOrWhiteSpace(datasetDto.Name))
            {
                throw new ArgumentException($"Field Dataset.Name is required. Current value: {datasetDto.Name}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Classification))
            {
                throw new ArgumentException($"Field Dataset.Classification is required. Current value: {datasetDto.Classification}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Location))
            {
                throw new ArgumentException($"Field Dataset.Location is required. Current value: {datasetDto.Location}");
            }
        }

        //public Task<StudyDto> UpdateDatasetAsync(int id, DatasetDto datasetToUpdate)
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}
