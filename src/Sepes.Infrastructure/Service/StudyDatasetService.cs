using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyDatasetService : DatasetServiceBase, IStudyDatasetService
    {
        readonly IConfiguration _config;
        readonly IAzureStorageAccountService _storageAccountService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureRoleAssignmentService _roleAssignmentService;
        readonly IDatasetFileService _datasetFileService;

        public StudyDatasetService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<StudyDatasetService> logger,
            IUserService userService,
            IAzureResourceGroupService resourceGroupService,
            IAzureStorageAccountService storageAccountService,
            IAzureRoleAssignmentService roleAssignmentService,
            IDatasetFileService datasetFileService)
            : base(db, mapper, logger, userService)
        {
            _config = config;
            _resourceGroupService = resourceGroupService;
            _storageAccountService = storageAccountService;
            _roleAssignmentService = roleAssignmentService;
            _datasetFileService = datasetFileService;
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

        public async Task<StudyDatasetDto> AddDatasetToStudyAsync(int studyId, int datasetId)
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

        public async Task RemoveDatasetFromStudyAsync(int studyId, int datasetId)
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
        }

        //STUDY SPECIFIC DATASETS

        public async Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputDto newDataset, CancellationToken cancellationToken = default)
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

            await CreateStorageAccountForStudySpecificDatasets(studyFromDb, dataset, cancellationToken);

            return await GetDatasetByStudyIdAndDatasetIdAsync(studyId, studyDataset.DatasetId);
        }

        async Task CreateStorageAccountForStudySpecificDatasets(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup))
            {
                study.StudySpecificDatasetsResourceGroup = AzureResourceNameUtil.StudySpecificDatasetResourceGroup(study.Name);
            }

            var tags = AzureResourceTagsFactory.StudySpecificDatasourceResourceGroupTags(_config, study);

            await _resourceGroupService.EnsureCreated(study.StudySpecificDatasetsResourceGroup, RegionStringConverter.Convert(dataset.Location), tags, cancellationToken);

            dataset.StorageAccountName = AzureResourceNameUtil.StudySpecificDataSetStorageAccount(dataset.StorageAccountName);

            var newStorageAccount = await _storageAccountService.CreateStorageAccount(RegionStringConverter.Convert(dataset.Location), study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, tags, cancellationToken);

            dataset.StorageAccountId = newStorageAccount.Id;
            dataset.StorageAccountName = newStorageAccount.Name;
            await _db.SaveChangesAsync();

            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            var roleAssignmentId = Guid.NewGuid().ToString();
            var roleDefinitionId = $"{dataset.StorageAccountId}/providers/Microsoft.Authorization/roleDefinitions/{AzureRoleDefinitionId.READ}";
            await _roleAssignmentService.AddResourceRoleAssignment(dataset.StorageAccountId, roleAssignmentId, roleDefinitionId, currentUser.ObjectId);
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

        public async Task SoftDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default)
        {
            await DeleteAllStudySpecificDatasetsWithHandlerAsync(study, SoftDeleteStudySpecificDatasetAsync, cancellationToken);
        }

        public async Task HardDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default)
        {
            await DeleteAllStudySpecificDatasetsWithHandlerAsync(study, HardDeleteStudySpecificDatasetAsync, cancellationToken);
        }

        async Task DeleteAllStudySpecificDatasetsWithHandlerAsync(Study study, Func<Study, int, CancellationToken, Task> deleteHandler, CancellationToken cancellationToken = default)
        {
            var studySpecificDatasetsToDelete = new List<int>();

            if (study.StudyDatasets.Any())
            {
                foreach (var studySpecificDataset in study.StudyDatasets.Where(sds => sds.Dataset.StudyId.HasValue && sds.Dataset.StudyId == study.Id))
                {
                    studySpecificDatasetsToDelete.Add(studySpecificDataset.DatasetId);
                }
            }

            await _db.SaveChangesAsync();

            if (studySpecificDatasetsToDelete.Any())
            {
                foreach (var curStudySpecificDatasetId in studySpecificDatasetsToDelete)
                {
                    var datasetToDelete = await _db.Datasets.Include(d => d.StudyDatasets).FirstOrDefaultAsync(d => d.Id == curStudySpecificDatasetId && d.StudyId.HasValue && d.StudyId == study.Id);

                    if (datasetToDelete != null)
                    {
                        await deleteHandler(study, datasetToDelete.Id, cancellationToken);
                    }
                }
            }
        }       

        public async Task SoftDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default)
        {
            var dataset = await GetStudySpecificDatasetOrThrowAsync(study.Id, datasetId, UserOperation.Study_AddRemove_Dataset);       
            await _storageAccountService.DeleteStorageAccount(study.StudySpecificDatasetsResourceGroup,dataset.StorageAccountName,  cancellationToken);
            await SoftDeleteAsync(dataset);
        }

        public async Task HardDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default)
        {
            var dataset = await GetStudySpecificDatasetOrThrowAsync(study.Id, datasetId, UserOperation.Study_AddRemove_Dataset);           
            await _storageAccountService.DeleteStorageAccount(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, cancellationToken);
            await HardDeleteAsync(dataset);
        }

        public string CalculateStorageAccountName(string userPrefix)
        {
            return AzureResourceNameUtil.StudySpecificDataSetStorageAccount(userPrefix);
        }
    }
}
