using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Storage;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetFileService : DatasetServiceBase, IDatasetFileService
    {
        readonly IAzureBlobStorageService _storageService;
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public DatasetFileService(SepesDbContext db, IMapper mapper, ILogger<DatasetFileService> logger, IUserService userService, IAzureBlobStorageService storageService, IDatasetCloudResourceService datasetCloudResourceService)
            : base(db, mapper, logger, userService)
        {
            _storageService = storageService;
            _datasetCloudResourceService = datasetCloudResourceService;
        }

        public async Task<List<BlobStorageItemDto>> AddFiles(int datasetId, List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Read, false);

                if (IsStudySpecific(dataset))
                {
                    //Verify access to study
                    var study = await GetStudyByIdAsync(dataset.StudyId.Value, UserOperation.Study_AddRemove_Dataset, false);
                    await _datasetCloudResourceService.EnsureExistFirewallExceptionForApplication(study, dataset, cancellationToken);
                    _storageService.SetResourceGroupAndAccountName(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName);
                }
                else
                {
                    throw new NotImplementedException("Only Study specific datasets is supported");
                    //ThrowIfOperationNotAllowed(Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);              
                }

                foreach (var curFile in files)
                {
                    await _storageService.UploadFileToBlobContainer(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, curFile.FileName, curFile, cancellationToken);
                }                

                return await GetFileList(datasetId, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to delete file from Storage Account - {ex.Message}", ex);
            }
        }

        public async Task DeleteFile(int datasetId, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Read, false);

                if (IsStudySpecific(dataset))
                {
                    //Verify access to study
                    var study = await GetStudyByIdAsync(dataset.StudyId.Value, UserOperation.Study_AddRemove_Dataset, false);
                    await _datasetCloudResourceService.EnsureExistFirewallExceptionForApplication(study, dataset, cancellationToken);
                    _storageService.SetResourceGroupAndAccountName(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName);
                }
                else
                {
                    throw new NotImplementedException("Only Study specific datasets is supported");
                    //ThrowIfOperationNotAllowed(Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);              
                }

                await _storageService.DeleteFileFromBlobContainer(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, fileName, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to delete file from Storage Account", ex);
            }

        }

        public async Task<List<BlobStorageItemDto>> GetFileList(int datasetId, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.Study_Read, false);

                if (IsStudySpecific(dataset))
                {
                    if (String.IsNullOrWhiteSpace(dataset.StorageAccountName))
                    {
                        throw new Exception($"Storage account name is null Dataset {dataset.Id}");
                    }

                    //Verify access to study
                    var study = await GetStudyByIdAsync(dataset.StudyId.Value, UserOperation.Study_Read, false);
                    await _datasetCloudResourceService.EnsureExistFirewallExceptionForApplication(study, dataset, cancellationToken);
                    _storageService.SetResourceGroupAndAccountName(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName);
                }
                else
                {
                    throw new NotImplementedException("Only Study specific datasets is supported");
                    //ThrowIfOperationNotAllowed(Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);              
                }

                return await _storageService.GetFileList(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, cancellationToken);

            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file list from Storage Account - {ex.Message}", ex);
            }
        }
    }
}
