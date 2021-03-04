using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class DatasetFactory
    {
        public static Dataset Create(
            CloudResource parentResourceGroup,
                string name,
                string location,
                string classification
              )
        {
            var dataset = DatasetBasic(parentResourceGroup, name, location, classification);
            return dataset;
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
