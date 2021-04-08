using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyDatasetService : DatasetServiceBase, IStudyDatasetService
    {
       readonly IStudyModelService _studyModelService;
        readonly IStudySpecificDatasetModelService _studySpecificDatasetModelService;

        public StudyDatasetService(SepesDbContext db, IMapper mapper, ILogger<StudyDatasetService> logger, IUserService userService, IStudyModelService studyModelService, IStudySpecificDatasetModelService studySpecificDatasetModelService)
            : base(db, mapper, logger, userService)
        {
            _studyModelService = studyModelService;
            _studySpecificDatasetModelService = studySpecificDatasetModelService;
        }

        public async Task<DatasetDto> GetDatasetByStudyIdAndDatasetIdAsync(int studyId, int datasetId)
        {           
            var studyFromDb = await _studyModelService.GetForDatasetsAsync(studyId);

            var studyDatasetRelation = studyFromDb.StudyDatasets.FirstOrDefault(sd => sd.DatasetId == datasetId);

            if (studyDatasetRelation == null)
            {
                throw NotFoundException.CreateForEntity("StudyDataset", datasetId);
            }

            var datasetDto = _mapper.Map<DatasetDto>(studyDatasetRelation.Dataset);
            await StudyPermissionsUtil.DecorateDtoStudySpecific(_userService, studyFromDb, datasetDto.Permissions);

            return datasetDto;
        }

        public async Task<IEnumerable<DatasetDto>> GetDatasetsForStudyAsync(int studyId)
        {
            var studyFromDb = await _studyModelService.GetForDatasetsAsync(studyId);           

            if (studyFromDb.StudyDatasets == null)
            {
                throw NotFoundException.CreateForEntityCustomDescr("StudyDatasets", $"studyId {studyId}");
            }

            var datasetDtos = _mapper.Map<IEnumerable<DatasetDto>>(studyFromDb.StudyDatasets);

            return datasetDtos;
        }

        public async Task<DatasetDto> AddPreApprovedDatasetToStudyAsync(int studyId, int datasetId)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await _studyModelService.GetForDatasetsAsync(studyId, UserOperation.Study_AddRemove_Dataset);
            var datasetFromDb = await _studySpecificDatasetModelService.GetByIdWithoutPermissionCheckAsync(datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            if (datasetFromDb.StudySpecific)
            {
                throw new ArgumentException($"Dataset {datasetId} is Study specific, and cannot be linked using this method.");
            }

            // Create new entry in linking table
            var studyDataset = new StudyDataset { Study = studyFromDb, Dataset = datasetFromDb };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            return _mapper.Map<DatasetDto>(studyDataset.Dataset);
        }     

        public async Task RemovePreApprovedDatasetFromStudyAsync(int studyId, int datasetId)
        {
            var studyFromDb = await _studyModelService.GetForDatasetsAsync(studyId, UserOperation.Study_AddRemove_Dataset);                  
          
            if (await _studySpecificDatasetModelService.IsStudySpecific(datasetId))
            {
                throw new Exception("Study specific datasets cannot be deleted using this method");              
            }
            else
            {
                //Get relation table entry
                var studyDatasetFromDb = await _db.StudyDatasets
                .FirstOrDefaultAsync(ds => ds.StudyId == studyId && ds.DatasetId == datasetId);

                //Is dataset really linked to this a study?
                if (studyDatasetFromDb == null)
                {
                    throw new NotFoundException("Dataset is not related to Study");
                }

                _db.StudyDatasets.Remove(studyDatasetFromDb);
                await _db.SaveChangesAsync();
            }
        } 
    }
}