using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyDatasetService : ServiceBase<Dataset>, IStudyDatasetService
    {
        readonly IStudyService _studyService;
        readonly IUserService _userService;

        public StudyDatasetService(SepesDbContext db, IMapper mapper, IStudyService studyService, IUserService userService)
            :base(db, mapper)
        {            
            _studyService = studyService;
            _userService = userService;
        }         


        public async Task<StudyDatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_Read, true);

            var studyDatasetRelation = studyFromDb.StudyDatasets.FirstOrDefault(sd => sd.DatasetId == datasetId);

            if (studyDatasetRelation == null)
            {
                throw NotFoundException.CreateForEntity("StudyDataset", datasetId);
            }

            var datasetDto = _mapper.Map<StudyDatasetDto>(studyDatasetRelation.Dataset);

            return datasetDto;
        }    

        public async Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Dataset); 
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            if (datasetFromDb.StudyId != null)
            {
                throw new ArgumentException($"Dataset with id {datasetId} is studySpecific, and cannot be linked using this method.");
            }

            // Create new entry in linking table
            var studyDataset = new StudyDataset { Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyDtoByIdAsync(studyId, UserOperation.Study_AddRemove_Dataset);
        }

        public async Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudy(int studyId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_Read, true);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Study", studyId);
            }

            if (studyFromDb.StudyDatasets == null)
            {
                throw NotFoundException.CreateForEntityCustomDescr("StudyDatasets", $"studyId {studyId}");
            }

            var datasetDtos = _mapper.Map<IEnumerable<StudyDatasetDto>>(studyFromDb.StudyDatasets);

            return datasetDtos;
        }

        public async Task<StudyDto> RemoveDatasetFromStudyAsync(int studyId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Dataset, true);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            //Does dataset exist?
            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            var studyDatasetFromDb = await _db.StudyDatasets
                .FirstOrDefaultAsync(ds => ds.StudyId == studyId && ds.DatasetId == datasetId);

            //Is dataset linked to a study?
            if (studyDatasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("StudyDataset", datasetId);
            }

            _db.StudyDatasets.Remove(studyDatasetFromDb);

            //If dataset is studyspecific, remove dataset as well.
            // Possibly keep database entry, but mark as deleted.
            if (datasetFromDb.StudyId != null)
            {
                _db.Datasets.Remove(datasetFromDb);
            }

            await _db.SaveChangesAsync();
            var retVal = await _studyService.GetStudyDtoByIdAsync(studyId, UserOperation.Study_AddRemove_Dataset);
            return retVal;
        }

        //STUDY SPECIFIC DATASETS

        public async Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputDto newDataset)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Dataset, true);
            DataSetUtils.PerformUsualTestForPostedDatasets(newDataset);
            var dataset = _mapper.Map<Dataset>(newDataset);
            dataset.StudyId = studyId;
            await _db.Datasets.AddAsync(dataset);

            // Create new linking table entry
            var studyDataset = new StudyDataset { Study = studyFromDb, Dataset = dataset };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await GetDatasetByStudyIdAndDatasetIdAsync(studyId, studyDataset.DatasetId);
        }   
      

        public async Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputDto updatedDataset)
        {
            DataSetUtils.PerformUsualTestForPostedDatasets(updatedDataset);

            var datasetFromDb = await GetStudySpecificDatasetOrThrowAsync(studyId, datasetId, UserOperation.Study_AddRemove_Dataset);

            DataSetUtils.UpdateDatasetBasicDetails(datasetFromDb, updatedDataset);            
           
            Validate(datasetFromDb);

            await _db.SaveChangesAsync();

            return await GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetFromDb.Id);
        }


        async Task<Dataset> GetStudySpecificDatasetOrThrowAsync(int studyId, int datasetId, UserOperation operation)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, operation, true);

            var studyDatasetRelation = studyFromDb.StudyDatasets.FirstOrDefault(sd => sd.DatasetId == datasetId);

            if (studyDatasetRelation == null)
            {
                throw NotFoundException.CreateForEntity("StudyDataset", datasetId);
            }

            return studyDatasetRelation.Dataset;
        }

      
    }
}
