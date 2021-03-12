using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Util
{
    public class DatasetUtilsTest
    {
        [InlineData("Field Dataset.Name is required. Current value: ", "", "classification", "location")]
        [InlineData("Field Dataset.Classification is required. Current value: ", "Name", "", "location")]
        [InlineData("Field Dataset.Location is required. Current value: ", "Name", "classification", "")]
        [Theory]
        public void PerformUsualTestForPostedDatasets_ShouldReturnCorrectErrorMessage(string expectedResult, string name, string Classification, string Location)
        {
            var ex = Assert.Throws<ArgumentException>(() => DatasetUtils.PerformUsualTestForPostedDatasets(new DatasetCreateUpdateInputBaseDto { Name = name, Classification = Classification, Location = Location }));

            Assert.Equal(expectedResult, ex.Message);
        }

        [InlineData("Name", "classification", "location")]
        [Theory]
        public void PerformUsualTestForPostedDatasets_ShouldNotReturn(string name, string Classification, string Location)
        {
            DatasetUtils.PerformUsualTestForPostedDatasets(new DatasetCreateUpdateInputBaseDto { Name = name, Classification = Classification, Location = Location });
        }

        [Fact]
        public void GetStudySpecificStorageAccountResourceEntry_ShouldReturnCorrectResource()
        {
            var resources = new List<CloudResource>();
            resources.Add(new CloudResource { ResourceType = AzureResourceType.StorageAccount, Purpose = CloudResourcePurpose.StudySpecificDatasetStorageAccount });
            var result = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(new Dataset{ StudySpecific = true, Resources = resources });

            Assert.Equal(AzureResourceType.StorageAccount, result.ResourceType);
        }

        [Fact]
        public void GetStudySpecificStorageAccountResourceEntry_ShouldNotReturn2()
        {
            var expectedResult = "Dataset is empty";
            var ex = Assert.Throws<ArgumentException>(() => DatasetUtils.GetStudySpecificStorageAccountResourceEntry(null));
            Assert.Equal(expectedResult, ex.Message);
        }
    }
}
