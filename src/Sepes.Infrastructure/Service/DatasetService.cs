using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetService : DatasetServiceBase, IDatasetService
    {       

        public DatasetService(SepesDbContext db, IMapper mapper, ILogger<DatasetService> logger, IUserService userService)
            : base(db, mapper, logger, userService)
        {
          
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

        public async Task<bool> IsStudySpecific(int datasetId)
        {
            var dataset = await GetDatasetOrThrowNoAccessCheckAsync(datasetId);
            return IsStudySpecific(dataset);
        }

        public async Task DeleteDatasetAsync(int datasetId)
        {
            throw new NotImplementedException("Delete of Pre-Approved datasets not implemented yet");
        }    
    }
}
