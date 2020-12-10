using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
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
    public class StudySpecificDatasetService : DatasetServiceBase, IStudySpecificDatasetService
    {
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public StudySpecificDatasetService(SepesDbContext db, IMapper mapper, ILogger<StudySpecificDatasetService> logger, IUserService userService, IDatasetCloudResourceService datasetCloudResourceService)
            : base(db, mapper, logger, userService)
        {
            _datasetCloudResourceService = datasetCloudResourceService ?? throw new ArgumentNullException(nameof(datasetCloudResourceService));
        }

        //public StudySpecificDatasetService(SepesDbContext db, IMapper mapper, ILogger<StudySpecificDatasetService> logger,
        //    IUserService userService,
        //    IDatasetCloudResourceService datasetCloudResourceService)
        //    : base(db, mapper, logger, userService)
        //{
        //    _datasetCloudResourceService = datasetCloudResourceService;           
        //}       



        public async Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputBaseDto newDatasetInput, CancellationToken cancellationToken = default)
        {            
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Dataset, true);

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(studyFromDb.WbsCode))
            {
                throw new Exception("WBS code missing in Study. Study requires WBS code before Dataset can be created.");
            }

            DataSetUtils.PerformUsualTestForPostedDatasets(newDatasetInput);
            var dataset = _mapper.Map<Dataset>(newDatasetInput);
            dataset.StudyId = studyId;
            dataset.StorageAccountName = AzureResourceNameUtil.StudySpecificDataSetStorageAccount(dataset.Name);

            var currentUser = _userService.GetCurrentUser();
            dataset.CreatedBy = currentUser.UserName;

            if (newDatasetInput.AllowAccessFromAddresses != null && newDatasetInput.AllowAccessFromAddresses.Count > 0)
            {
                dataset.FirewallRules = new List<DatasetFirewallRule>();

                foreach (var curNewAddress in newDatasetInput.AllowAccessFromAddresses)
                {
                    dataset.FirewallRules.Add(new DatasetFirewallRule() { CreatedBy = currentUser.UserName, Address = curNewAddress });
                }
            }

            await _db.Datasets.AddAsync(dataset);

            // Create new linking table entry
            var studyDataset = new StudyDataset { Study = studyFromDb, Dataset = dataset };
            await _db.StudyDatasets.AddAsync(studyDataset);
            await _db.SaveChangesAsync();

            await _datasetCloudResourceService.CreateResourcesForStudySpecificDatasetAsync(studyFromDb, dataset, cancellationToken);

            return _mapper.Map<StudyDatasetDto>(dataset);
        }       

        public async Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputBaseDto updatedDataset)
        {
            DataSetUtils.PerformUsualTestForPostedDatasets(updatedDataset);

            var datasetFromDb = await GetStudySpecificDatasetOrThrowAsync(studyId, datasetId, UserOperation.Study_AddRemove_Dataset);

            DataSetUtils.UpdateDatasetBasicDetails(datasetFromDb, updatedDataset);

            Validate(datasetFromDb);

            await _db.SaveChangesAsync();

            return _mapper.Map<StudyDatasetDto>(datasetFromDb);
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

        public async Task SoftDeleteStudySpecificDatasetAsync(int datasetId, CancellationToken cancellationToken = default)
        {
            var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.Study_AddRemove_Dataset, false);
            var study = dataset.StudyDatasets.SingleOrDefault().Study;
            await _datasetCloudResourceService.DeleteResourcesForStudySpecificDatasetAsync(study, dataset, cancellationToken);
            await SoftDeleteAsync(dataset);
        }

        public async Task SoftDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default)
        {
            var dataset = await GetStudySpecificDatasetOrThrowAsync(study.Id, datasetId, UserOperation.Study_AddRemove_Dataset);       
            await _datasetCloudResourceService.DeleteResourcesForStudySpecificDatasetAsync(study, dataset, cancellationToken);
            await SoftDeleteAsync(dataset);
        }

        public async Task HardDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default)
        {
            var dataset = await GetStudySpecificDatasetOrThrowAsync(study.Id, datasetId, UserOperation.Study_AddRemove_Dataset);
            await _datasetCloudResourceService.DeleteResourcesForStudySpecificDatasetAsync(study, dataset, cancellationToken);
            await HardDeleteAsync(dataset);
        }

        public async Task HardDeleteStudySpecificDatasetAsync(int datasetId, CancellationToken cancellationToken = default)
        {
            var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.Study_AddRemove_Dataset, false);
            var study = dataset.StudyDatasets.SingleOrDefault().Study;
            await _datasetCloudResourceService.DeleteResourcesForStudySpecificDatasetAsync(study, dataset, cancellationToken);
            await HardDeleteAsync(dataset);
        }

        public async Task DeleteAllStudyRelatedResourcesAsync(Study study, CancellationToken cancellationToken = default)
        {
            await _datasetCloudResourceService.DeleteAllStudyRelatedResourcesAsync(study, cancellationToken);
        }
    }
}