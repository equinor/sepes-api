using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Storage;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetFileService : DatasetServiceBase, IDatasetFileService
    {
        readonly IAzureBlobStorageService _storageService;
        readonly IAzureStorageAccountTokenService _azureStorageAccountTokenService;
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public DatasetFileService(SepesDbContext db, IMapper mapper, ILogger<DatasetFileService> logger, IUserService userService,
            IAzureBlobStorageService storageService,
            IDatasetCloudResourceService datasetCloudResourceService,
            IAzureStorageAccountTokenService azureStorageAccountTokenService)
            : base(db, mapper, logger, userService)
        {
            _storageService = storageService;
            _datasetCloudResourceService = datasetCloudResourceService;
            _azureStorageAccountTokenService = azureStorageAccountTokenService;
        }

        public async Task<List<BlobStorageItemDto>> AddFiles(int datasetId, List<IFormFile> files, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Read, false);

                if (IsStudySpecific(dataset))
                {
                    //Verify access to study
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);
                    await ThrowIfOperationNotAllowed(UserOperation.Study_AddRemove_Dataset, study);
                
                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);
                
                    _storageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
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

                return await GetFileListAsync(datasetId, clientIp, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to delete file from Storage Account - {ex.Message}", ex);
            }
        }     

        public async Task<List<BlobStorageItemDto>> GetFileListAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.Study_Read, false);

                if (IsStudySpecific(dataset))
                {
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);

                    if (datasetResourceEntry == null)
                    {
                        throw new Exception($"Resource entry for Dataset {dataset.Id} not found");
                    }

                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);
                    await ThrowIfOperationNotAllowed(UserOperation.Study_Read, study);
                
                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    _storageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
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


        public async Task<string> GetFileUploadUriBuilderWithSasTokenAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Read, false);

                if (IsStudySpecific(dataset))
                {
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);

                    //Verify access to study
                    await ThrowIfOperationNotAllowed(UserOperation.Study_AddRemove_Dataset, study);               

                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);

                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    _storageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                    await _storageService.EnsureContainerExist(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, cancellationToken);
                  
                    _azureStorageAccountTokenService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }

                var uriBuilder = await _azureStorageAccountTokenService.CreateFileUploadUriBuilder("files", cancellationToken);

                return uriBuilder.Uri.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file list from Storage Account - {ex.Message}", ex);
            }
        }

        public async Task<string> GetFileDeleteUriBuilderWithSasTokenAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                var dataset = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Read, false);

                if (IsStudySpecific(dataset))
                {
                    //Verify access to study
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);
                    await ThrowIfOperationNotAllowed(UserOperation.Study_AddRemove_Dataset, study);
                
                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);
                    _azureStorageAccountTokenService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }

                var uriBuilder = await _azureStorageAccountTokenService.CreateFileDeleteUriBuilder("files", cancellationToken);

                return uriBuilder.Uri.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file list from Storage Account - {ex.Message}", ex);
            }
        }     
    }
}