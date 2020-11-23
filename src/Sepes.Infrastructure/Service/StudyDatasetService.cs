using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
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
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyRead, true);

            var studyDatasetRelation = studyFromDb.StudyDatasets.FirstOrDefault(sd => sd.DatasetId == datasetId);

            if (studyDatasetRelation == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            var datasetDto = _mapper.Map<StudyDatasetDto>(studyDatasetRelation.Dataset);

            return datasetDto;
        }

        async Task<Dataset> GetDatasetOrThrowAsync(int id)
        {
            var datasetFromDb = await _db.Datasets
                .Where(ds => ds.StudyId == null)
                .Include(s => s. StudyDatasets)
                .ThenInclude(sd=> sd.Study)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", id);
            }

            return datasetFromDb;
        }

        public async Task<StudyDto> AddDatasetToStudyAsync(int studyId, int datasetId)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveDataset); 
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            if (datasetFromDb.StudyId != null)
            {
                throw new ArgumentException($"Dataset with id {datasetId} is studySpecific, and cannot be linked using this method.");
            }

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset { Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyDtoByIdAsync(studyId, Constants.UserOperations.StudyAddRemoveDataset);
        }

        public async Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudy(int studyId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.SandboxEdit, true);

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
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveDataset, true);
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
            var retVal = await _studyService.GetStudyDtoByIdAsync(studyId, Constants.UserOperations.StudyAddRemoveDataset);
            return retVal;
        }

        public async Task<StudyDatasetDto> AddStudySpecificDatasetAsync(int studyId, StudySpecificDatasetDto newDataset)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveDataset, true);
            PerformUsualTestForPostedDatasets(newDataset);
            var dataset = _mapper.Map<Dataset>(newDataset);
            dataset.StudyId = studyId;
            await _db.Datasets.AddAsync(dataset);

            // Create new linking table
            StudyDataset studyDataset = new StudyDataset { Study = studyFromDb, Dataset = dataset };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return await GetDatasetByStudyIdAndDatasetIdAsync(studyId, studyDataset.DatasetId);
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
            if (String.IsNullOrWhiteSpace(datasetDto.StorageAccountName))
            {
                throw new ArgumentException($"Field Dataset.StorageAccountName is required. Current value: {datasetDto.StorageAccountName}");
            }
        }      
      

        public async Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, StudySpecificDatasetDto updatedDataset)
        {
            PerformUsualTestForPostedDatasets(updatedDataset);
            var datasetFromDb = await GetStudySpecificDatasetOrThrowAsync(studyId, datasetId);
            if (!String.IsNullOrWhiteSpace(updatedDataset.Name) && updatedDataset.Name != datasetFromDb.Name)
            {
                datasetFromDb.Name = updatedDataset.Name;
            }
            if (!String.IsNullOrWhiteSpace(updatedDataset.Location) && updatedDataset.Location != datasetFromDb.Location)
            {
                datasetFromDb.Location = updatedDataset.Location;
            }
            if (!String.IsNullOrWhiteSpace(updatedDataset.Classification) && updatedDataset.Classification != datasetFromDb.Classification)
            {
                datasetFromDb.Classification = updatedDataset.Classification;
            }
            //if (!String.IsNullOrWhiteSpace(updatedDataset.StorageAccountName) && updatedDataset.StorageAccountName != datasetFromDb.StorageAccountName)
            //{
            //    datasetFromDb.StorageAccountName = updatedDataset.StorageAccountName;
            //}
            if (updatedDataset.LRAId != datasetFromDb.LRAId)
            {
                datasetFromDb.LRAId = updatedDataset.LRAId;
            }
            if (updatedDataset.DataId != datasetFromDb.DataId)
            {
                datasetFromDb.DataId = updatedDataset.DataId;
            }
            if (updatedDataset.SourceSystem != datasetFromDb.SourceSystem)
            {
                datasetFromDb.SourceSystem = updatedDataset.SourceSystem;
            }
            if (updatedDataset.BADataOwner != datasetFromDb.BADataOwner)
            {
                datasetFromDb.BADataOwner = updatedDataset.BADataOwner;
            }
            if (updatedDataset.Asset != datasetFromDb.Asset)
            {
                datasetFromDb.Asset = updatedDataset.Asset;
            }
            if (updatedDataset.CountryOfOrigin != datasetFromDb.CountryOfOrigin)
            {
                datasetFromDb.CountryOfOrigin = updatedDataset.CountryOfOrigin;
            }
            if (updatedDataset.AreaL1 != datasetFromDb.AreaL1)
            {
                datasetFromDb.AreaL1 = updatedDataset.AreaL1;
            }
            if (updatedDataset.AreaL2 != datasetFromDb.AreaL2)
            {
                datasetFromDb.AreaL2 = updatedDataset.AreaL2;
            }
            if (updatedDataset.Tags != datasetFromDb.Tags)
            {
                datasetFromDb.Tags = updatedDataset.Tags;
            }
            if (updatedDataset.Description != datasetFromDb.Description)
            {
                datasetFromDb.Description = updatedDataset.Description;
            }
            Validate(datasetFromDb);
            await _db.SaveChangesAsync();
            return await GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetFromDb.Id);
        }

        async Task<Dataset> GetStudySpecificDatasetOrThrowAsync(int studyId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyRead, true);

            var studyDatasetRelation = studyFromDb.StudyDatasets.FirstOrDefault(sd => sd.DatasetId == datasetId);

            if (studyDatasetRelation == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            return studyDatasetRelation.Dataset;
        }

      
    }
}
