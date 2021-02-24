using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Constants;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class SandboxOperations
    {
        public static async Task<AddDatasetToSandboxResult> AddDataset(RestHelper restHelper, int sandboxId, int datasetId)         
        {       
            var response = await restHelper.Put<AvailableDatasets>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));

            return new AddDatasetToSandboxResult(response);
        }
    }

    public class AddDatasetToSandboxResult
    {
        public AddDatasetToSandboxResult(ApiResponseWrapper<AvailableDatasets> response)
        {          
            Response = response;
        }     

        public ApiResponseWrapper<AvailableDatasets> Response { get; private set; }       
    }
}
