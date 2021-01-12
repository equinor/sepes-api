using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Model;
using System;

namespace Sepes.Infrastructure.Util
{
    public static class DataSetUtils
    {

        public static void UpdateDatasetBasicDetails(Dataset datasetFromDb, DatasetCreateUpdateInputBaseDto updatedDataset)
        {

            if (!String.IsNullOrWhiteSpace(updatedDataset.Name) && updatedDataset.Name != datasetFromDb.Name)
            {
                datasetFromDb.Name = updatedDataset.Name;
            }         

            if (!String.IsNullOrWhiteSpace(updatedDataset.Classification) && updatedDataset.Classification != datasetFromDb.Classification)
            {
                datasetFromDb.Classification = updatedDataset.Classification;
            }

            if (updatedDataset.DataId != 0 && updatedDataset.DataId != datasetFromDb.DataId)
            {
                datasetFromDb.DataId = updatedDataset.DataId;
            }
        }
        /*
        public static void UpdateDatasetExtendedDetails(Dataset datasetToUpdate, DatasetDto details)
        {
            if (details.LRAId != datasetToUpdate.LRAId)
            {
                datasetToUpdate.LRAId = details.LRAId;
            }
            if (details.DataId != datasetToUpdate.DataId)
            {
                datasetToUpdate.DataId = details.DataId;
            }
            if (details.SourceSystem != datasetToUpdate.SourceSystem)
            {
                datasetToUpdate.SourceSystem = details.SourceSystem;
            }
            if (details.BADataOwner != datasetToUpdate.BADataOwner)
            {
                datasetToUpdate.BADataOwner = details.BADataOwner;
            }
            if (details.Asset != datasetToUpdate.Asset)
            {
                datasetToUpdate.Asset = details.Asset;
            }
            if (details.CountryOfOrigin != datasetToUpdate.CountryOfOrigin)
            {
                datasetToUpdate.CountryOfOrigin = details.CountryOfOrigin;
            }
            if (details.AreaL1 != datasetToUpdate.AreaL1)
            {
                datasetToUpdate.AreaL1 = details.AreaL1;
            }
            if (details.AreaL2 != datasetToUpdate.AreaL2)
            {
                datasetToUpdate.AreaL2 = details.AreaL2;
            }
            if (details.Tags != datasetToUpdate.Tags)
            {
                datasetToUpdate.Tags = details.Tags;
            }
            if (details.Description != datasetToUpdate.Description)
            {
                datasetToUpdate.Description = details.Description;
            }
        }
        */

        public static void PerformUsualTestForPostedDatasets(DatasetCreateUpdateInputBaseDto datasetDto)
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
            //if (String.IsNullOrWhiteSpace(datasetDto.StorageAccountName))
            //{
            //    throw new ArgumentException($"Field Dataset.StorageAccountName is required. Current value: {datasetDto.StorageAccountName}");
            //}
        }
    }

}
