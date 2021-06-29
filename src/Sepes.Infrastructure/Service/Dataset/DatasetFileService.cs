using AutoMapper;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Storage;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model.Context;
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
        readonly IAzureBlobStorageService _azureBlobStorageService;
        readonly IStudySpecificDatasetModelService _studySpecificDatasetModelService;
        readonly IAzureBlobStorageUriBuilderService _azureBlobStorageUriBuilderService;
        readonly IDatasetCloudResourceService _datasetCloudResourceService;

        public DatasetFileService(SepesDbContext db, IMapper mapper, ILogger<DatasetFileService> logger, IUserService userService,
            IStudyPermissionService studyPermissionService,
            IStudySpecificDatasetModelService studySpecificDatasetModelService,
            IAzureBlobStorageService azureBlobStorageService,
            IDatasetCloudResourceService datasetCloudResourceService,
            IAzureBlobStorageUriBuilderService azureBlobStorageUriBuilderService)
            : base(db, mapper, logger, userService, studyPermissionService)
        {
            _azureBlobStorageService = azureBlobStorageService;
            _studySpecificDatasetModelService = studySpecificDatasetModelService;
            _datasetCloudResourceService = datasetCloudResourceService;
            _azureBlobStorageUriBuilderService = azureBlobStorageUriBuilderService;
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
                    _azureBlobStorageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }
                else
                {
                    throw new NotImplementedException("Only Study specific datasets is supported");
                }

                return await _azureBlobStorageService.GetFileList(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, cancellationToken);

            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file list from Storage Account - {ex.Message}", ex);
            }
        }


        public async Task<string> GetFileUploadUriAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await _studySpecificDatasetModelService.IsStudySpecific(datasetId))
                {
                    var dataset = await _studySpecificDatasetModelService.GetForResourceAndFirewall(datasetId, UserOperation.Study_AddRemove_Dataset);
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);

                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);

                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    _azureBlobStorageService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                    await _azureBlobStorageService.EnsureContainerExist(DatasetConstants.STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER, cancellationToken);

                    _azureBlobStorageUriBuilderService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }

                var uriBuilder = await CreateFileUploadUriBuilder("files", cancellationToken);

                return uriBuilder.Uri.ToString();

            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file upload url builder from Storage Account - {ex.Message}", ex);
            }
        }     

        async Task<UriBuilder> CreateFileUploadUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await _azureBlobStorageUriBuilderService.CreateUriBuilder(containerName, permission: BlobContainerSasPermissions.Write, expiresOnMinutes: 30, cancellationToken: cancellationToken);
        }

        public async Task<string> GetFileDeleteUriAsync(int datasetId, string clientIp, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await _studySpecificDatasetModelService.IsStudySpecific(datasetId))
                {
                    var dataset = await _studySpecificDatasetModelService.GetForResourceAndFirewall(datasetId, UserOperation.Study_AddRemove_Dataset);
                    var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);

                    await _datasetCloudResourceService.EnsureFirewallExistsAsync(study, dataset, clientIp, cancellationToken);
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(dataset);
                    _azureBlobStorageUriBuilderService.SetConnectionParameters(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName);
                }

                var uriBuilder = await CreateFileDeleteUriBuilder("files", cancellationToken);

                return uriBuilder.Uri.ToString();

            }
            catch (ForbiddenException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to get file delete url builder from Storage Account - {ex.Message}", ex);
            }
        }

        public async Task<UriBuilder> CreateFileDeleteUriBuilder(string containerName, CancellationToken cancellationToken = default)
        {
            return await _azureBlobStorageUriBuilderService.CreateUriBuilder(containerName, permission: BlobContainerSasPermissions.Delete, expiresOnMinutes: 5, cancellationToken: cancellationToken);
        }
    }
}