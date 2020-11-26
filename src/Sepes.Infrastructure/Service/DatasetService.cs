﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetService : ServiceBase<Dataset>, IDatasetService
    {
        readonly IUserService _userService;

        public DatasetService(SepesDbContext db, IMapper mapper, IUserService userService)
            : base(db, mapper)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<DatasetListItemDto>> GetDatasetsLookupAsync()
        {
            ThrowIfOperationNotAllowed(UserOperation.PreApprovedDataset_Read);

            var datasetsFromDb = await _db.Datasets
                .Where(ds => ds.StudyId == null)
                .ToListAsync();
            var dataasetsDtos = _mapper.Map<IEnumerable<DatasetListItemDto>>(datasetsFromDb);

            return dataasetsDtos;
        }

        public async Task<IEnumerable<DatasetDto>> GetDatasetsAsync()
        {
            ThrowIfOperationNotAllowed(UserOperation.PreApprovedDataset_Read);

            var datasetsFromDb = await _db.Datasets
                .Where(ds => ds.StudyId == null)
                .ToListAsync();
            var dataasetDtos = _mapper.Map<IEnumerable<DatasetDto>>(datasetsFromDb);

            return dataasetDtos;
        }

        public async Task<DatasetDto> GetDatasetByDatasetIdAsync(int datasetId)
        {
            var datasetFromDb = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Read);

            var datasetDto = _mapper.Map<DatasetDto>(datasetFromDb);

            return datasetDto;
        }

        async Task<Dataset> GetDatasetOrThrowAsync(int id, UserOperation operation)
        {
            var datasetFromDb = await _db.Datasets
                .Where(ds => ds.StudyId == null)
                .Include(s => s.StudyDatasets)
                .ThenInclude(sd => sd.Study)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", id);
            }

            ThrowIfOperationNotAllowed(operation);

            return datasetFromDb;
        }

        void ThrowIfOperationNotAllowed(UserOperation operation)
        {
            if (StudyAccessUtil.HasAccessToOperation(_userService, operation) == false)
            {
                throw new ForbiddenException($"User {_userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation}");
            }
        }


        void PerformUsualTestForPostedDatasets(DatasetDto datasetDto)
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
        }

        public async Task<DatasetDto> CreateDatasetAsync(DatasetDto newDataset)
        {
            ThrowIfOperationNotAllowed(UserOperation.PreApprovedDataset_Create_Update_Delete);

            var newDatasetDbModel = _mapper.Map<Dataset>(newDataset);
            var newDatasetId = await Add(newDatasetDbModel);
            return await GetDatasetByDatasetIdAsync(newDatasetId);
        }

        public async Task<DatasetDto> UpdateDatasetAsync(int datasetId, DatasetDto updatedDataset)
        {
            var datasetFromDb = await GetDatasetOrThrowAsync(datasetId, UserOperation.PreApprovedDataset_Create_Update_Delete);

            PerformUsualTestForPostedDatasets(updatedDataset);

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
            if (!String.IsNullOrWhiteSpace(updatedDataset.StorageAccountName) && updatedDataset.StorageAccountName != datasetFromDb.StorageAccountName)
            {
                datasetFromDb.StorageAccountName = updatedDataset.StorageAccountName;
            }
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
            datasetFromDb.Updated = DateTime.UtcNow;
            Validate(datasetFromDb);
            await _db.SaveChangesAsync();
            return await GetDatasetByDatasetIdAsync(datasetFromDb.Id);
        }
    }
}
