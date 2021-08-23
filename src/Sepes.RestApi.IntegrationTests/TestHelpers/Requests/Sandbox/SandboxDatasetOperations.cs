using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class SandboxDatasetOperations
    {
        public static async Task<AddorRemoveDatasetToSandboxResult> AddDatasetExpectSuccess(RestHelper restHelper, int sandboxId, int datasetId)         
        {       
            var response = await restHelper.Put<AvailableDatasets>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));
            return new AddorRemoveDatasetToSandboxResult(response);
        }

        public static async Task<AddorRemoveDatasetToSandboxResult> RemoveDatasetExpectSuccess(RestHelper restHelper, int sandboxId, int datasetId)
        {
            var response = await restHelper.Delete<AvailableDatasets>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));
            return new AddorRemoveDatasetToSandboxResult(response);
        }

        public static async Task<ApiConversation<Common.Response.ErrorResponse>> AddDatasetExpectFailure(RestHelper restHelper, int sandboxId, int datasetId)
        {
            var response = await restHelper.Put<Common.Response.ErrorResponse>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));
            return new ApiConversation<Common.Response.ErrorResponse>(response);
        }

        public static async Task<ApiConversation<Common.Response.ErrorResponse>> RemoveDatasetExpectFailure(RestHelper restHelper, int sandboxId, int datasetId)
        {
            var response = await restHelper.Delete<Common.Response.ErrorResponse>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));
            return new ApiConversation<Common.Response.ErrorResponse>(response);
        }       
    }

    public class AddorRemoveDatasetToSandboxResult
    {
        public AddorRemoveDatasetToSandboxResult(ApiResponseWrapper<AvailableDatasets> response)
        {          
            Response = response;
        }     

        public ApiResponseWrapper<AvailableDatasets> Response { get; private set; }       
    }
}
