using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Dto;
using System.Linq;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class AddDatasetToSandboxAsserts
    {
        public static void ExpectSuccess(int datasetId, string datasetName, string classification, string totalClassification, ApiResponseWrapper<AvailableDatasets> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<AvailableDatasets>(responseWrapper);

            Assert.NotEmpty(responseWrapper.Response.Datasets);
            Assert.Equal(totalClassification, responseWrapper.Response.Classification);

            var newlyAddedDataset = responseWrapper.Response.Datasets.SingleOrDefault(ds => ds.DatasetId == datasetId);

            Assert.NotNull(newlyAddedDataset);

            Assert.Equal(datasetId, newlyAddedDataset.DatasetId);
            Assert.Equal(datasetName, newlyAddedDataset.Name);
            Assert.Equal(classification, newlyAddedDataset.Classification);
            Assert.True(newlyAddedDataset.AddedToSandbox);

        }
    }
}
