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

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> AddDatasetExpectFailure(RestHelper restHelper, int sandboxId, int datasetId)
        {
            var response = await restHelper.Put<Common.Dto.ErrorResponse>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));
            return new ApiConversation<Common.Dto.ErrorResponse>(response);
        }

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> RemoveDatasetExpectFailure(RestHelper restHelper, int sandboxId, int datasetId)
        {
            var response = await restHelper.Delete<Common.Dto.ErrorResponse>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));
            return new ApiConversation<Common.Dto.ErrorResponse>(response);
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
