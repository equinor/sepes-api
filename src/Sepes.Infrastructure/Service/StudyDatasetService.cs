using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyDatasetService : DatasetServiceBase, IStudyDatasetService
    {
        readonly IConfiguration _config;
        readonly IAzureStorageAccountService _storageAccountService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureRoleAssignmentService _roleAssignmentService;

        public StudyDatasetService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<StudyDatasetService> logger,
            IUserService userService,
            IAzureResourceGroupService resourceGroupService,
            IAzureStorageAccountService storageAccountService,
            IAzureRoleAssignmentService roleAssignmentService)
            : base(db, mapper, logger, userService)
        {
            _config = config;
            _resourceGroupService = resourceGroupService;
            _storageAccountService = storageAccountService;
            _roleAssignmentService = roleAssignmentService;
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

        public async Task<StudyDatasetDto> AddPreApprovedDatasetToStudyAsync(int studyId, int datasetId)
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

            return _mapper.Map<StudyDatasetDto>(studyDataset.Dataset);
        }

        public async Task<IEnumerable<StudyDatasetDto>> GetDatasetsForStudyAsync(int studyId)
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

        public async Task RemovePreApprovedDatasetFromStudyAsync(int studyId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Dataset, true);
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            //Does dataset exist?
            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }
          
            if (datasetFromDb.StudyId != null)
            {
                throw new Exception("Study specific datasets cannot be deleted using this method");              
            }
            else
            {
                var studyDatasetFromDb = await _db.StudyDatasets
                .FirstOrDefaultAsync(ds => ds.StudyId == studyId && ds.DatasetId == datasetId);

                //Is dataset linked to a study?
                if (studyDatasetFromDb == null)
                {
                    throw NotFoundException.CreateForEntity("StudyDataset", datasetId);
                }

                _db.StudyDatasets.Remove(studyDatasetFromDb);
                await _db.SaveChangesAsync();
            }
        }

 
    }
}