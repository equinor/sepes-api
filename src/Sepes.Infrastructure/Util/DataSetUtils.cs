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
