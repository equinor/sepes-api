using AutoMapper;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Storage;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetFileService : DatasetServiceBase, IDatasetFileService
    {
        readonly IAzureBlobStorageService _storageService;
        readonly IStudySpecificDatasetModelService _studySpecificDatasetModelService;
        readonly IAzureBlobStorageUriBuilderService _azureStorageAccountTokenService;
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public DatasetFileService(SepesDbContext db, IMapper mapper, ILogger<DatasetFileService> logger, IUserService userService, IStudySpecificDatasetModelService studySpecificDatasetModelService, 
            IAzureBlobStorageService storageService,
            IDatasetCloudResourceService datasetCloudResourceService,
            IAzureBlobStorageUriBuilderService azureStorageAccountTokenService)
            : base(db, mapper, logger, userService)
        {
            _storageService = storageService;
            _studySpecificDatasetModelService = studySpecificDatasetModelService;
            _datasetCloudResourceService = datasetCloudResourceService;
            _azureStorageAccountTokenService = azureStorageAccountTokenService;
        }      

        public async Task<List<BlobStorageItemDto>> GetFileListAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await _studySpecificDatasetModelService.IsStudySpecific(datasetId))
                {
                    var dataset = await _studySpecificDatasetModelService.GetForResourceAndFirewall(datasetId, UserOperation.Study_Read);
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);

                    if (datasetResourceEntry == null)
                    {
                        throw new Exception($"Resource entry for Dataset {dataset.Id} not found");
                    }

                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);                   
                
                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    _storageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }
                else
                {
                    throw new NotImplementedException("Only Study specific datasets is supported");                            
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
                if (await _studySpecificDatasetModelService.IsStudySpecific(datasetId))
                {
                    var dataset = await _studySpecificDatasetModelService.GetForResourceAndFirewall(datasetId, UserOperation.Study_AddRemove_Dataset);
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);                            

                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);

                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    _storageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                    await _storageService.EnsureContainerExist(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, cancellationToken);
                  
                    _azureStorageAccountTokenService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }

                var uriBuilder = await CreateFileUploadUriBuilder("files", cancellationToken);

                return uriBuilder.Uri.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file list from Storage Account - {ex.Message}", ex);
            }
        }

        async Task<UriBuilder> CreateFileUploadUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await _azureStorageAccountTokenService.CreateUriBuilder(containerName, permission: BlobContainerSasPermissions.Write, expiresOnMinutes: 30, cancellationToken: cancellationToken);
        }

        public async Task<string> GetFileDeleteUriBuilderWithSasTokenAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            { 
                if (await _studySpecificDatasetModelService.IsStudySpecific(datasetId))
                {
                    var dataset = await _studySpecificDatasetModelService.GetForResourceAndFirewall(datasetId, UserOperation.Study_AddRemove_Dataset);
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);                 
                
                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);
                    _azureStorageAccountTokenService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }

                var uriBuilder = await CreateFileDeleteUriBuilder("files", cancellationToken);

                return uriBuilder.Uri.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file list from Storage Account - {ex.Message}", ex);
            }
        }

        public async Task<UriBuilder> CreateFileDeleteUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await _azureStorageAccountTokenService.CreateUriBuilder(containerName, permission: BlobContainerSasPermissions.Delete, expiresOnMinutes: 5, cancellationToken: cancellationToken);
        }
    }
}