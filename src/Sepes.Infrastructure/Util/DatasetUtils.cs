using Sepes.Common.Constants;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Model;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class DatasetUtils
    {
        public static Study GetStudyFromStudySpecificDatasetOrThrow(Dataset dataset)
        {
            if (dataset == null)
            {
                throw new ArgumentException("Dataset is empty");
            }

            var studyDatasetRelation = dataset.StudyDatasets.SingleOrDefault();

            if (studyDatasetRelation == null)
            {
                throw new Exception("GetStudyFromStudySpecificDatasetOrThrow: Dataset appears to be study specific, but no relation found");
            }

            if (studyDatasetRelation.Study == null)
            {
                throw new Exception("GetStudyFromStudySpecificDatasetOrThrow: Missing include on Study");
            }

            return studyDatasetRelation.Study;
        }

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
        }

        public static CloudResource GetStudySpecificStorageAccountResourceEntry(Dataset dataset)
        {
            if (dataset == null)
            {
                throw new ArgumentException("Dataset is empty");
            }

            if(dataset.StudySpecific)
            {
                if(dataset.Resources == null)
                {
                    return null;
                }
                else
                {
                    return dataset.Resources.SingleOrDefault(r => r.ResourceType == AzureResourceType.StorageAccount && r.Purpose == CloudResourcePurpose.StudySpecificDatasetStorageAccount);
                }
             
            }

            throw new ArgumentException("Only supports Study Specific Dataset");
        }

   

    }
}
