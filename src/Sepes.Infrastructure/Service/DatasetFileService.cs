using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetFileService : DatasetServiceBase, IDatasetFileService
    {
        readonly IAzureBlobStorageService _storageService;

        public DatasetFileService(SepesDbContext db, IMapper mapper, ILogger<DatasetFileService> logger, IUserService userService, IAzureBlobStorageService storageService)
            : base(db, mapper, logger, userService)
        {
            _storageService = storageService;
        }


        public async Task<List<Guid>> AddFiles(int datasetId, List<IFormFile> files)
        {
            var dataset = await GetDatasetOrThrowAsync(datasetId, Constants.UserOperation.PreApprovedDataset_Read);

            if (dataset.StudyId.HasValue)
            {
                var study = await GetStudyByIdAsync(dataset.StudyId.Value, Constants.UserOperation.Study_AddRemove_Dataset, false);
            }
            else
            {
                ThrowIfOperationNotAllowed(Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);
            }          

            //TODO: Verify format of this
            _storageService.SetAccountUrl(dataset.StorageAccountName);
            //get hold of relevant storage account
            //get hold of relevant container
            //ensure account and container exist, thow if account does not exist
            //Upload files
            //Set permissions

            return new List<Guid>();

        }
    }
}
