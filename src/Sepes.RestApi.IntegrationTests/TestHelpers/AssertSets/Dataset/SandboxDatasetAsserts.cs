using Sepes.Common.Dto.Dataset;
using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset
{
    public static class SandboxDatasetAsserts
    {
        public static void NewlyAddedExpectSuccess(AddorRemoveDatasetToSandboxResult result)
        {
            ApiResponseBasicAsserts.ExpectSuccess<AvailableDatasets>(result.Response);  
       
        }

        public static void NewlyRemovedExpectSuccess(AddorRemoveDatasetToSandboxResult result)
        {
            ApiResponseBasicAsserts.ExpectSuccess<AvailableDatasets>(result.Response);    
        }
    }
}
