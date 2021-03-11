using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class DatasetFactory
    {     

        public static StudyDataset CreateStudySpecificRelation(
            Study study,
             string name,
             string location,
             string classification
           )
        {
            var parentResourceGroup = study.Resources.FirstOrDefault();
            var dataset = DatasetBasic(parentResourceGroup, name, location, classification);           
            var relation = new StudyDataset() { StudyId = study.Id, Dataset = dataset };      

            return relation;
        }

        static Dataset DatasetBasic(CloudResource parentResourceGroup, string name, string location, string classification)
        {
            var storageAccountName = AzureResourceNameUtil.StudySpecificDataSetStorageAccount(name);

            var storageAccountResource = CloudResourceFactory.Create(location, AzureResourceType.StorageAccount, parentResourceGroup.ResourceName, storageAccountName, purpose: CloudResourcePurpose.StudySpecificDatasetStorageAccount);          
            storageAccountResource.ParentResource = parentResourceGroup;

            var dataset = new Dataset()
            {
                Name = name,
                Description = name + " description",
                Location = location,
                Classification = classification,

                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow,
                Resources = new List<CloudResource>() {
              storageAccountResource
                }
            };

            return dataset;
        }

    }
}
