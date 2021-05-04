using Sepes.Infrastructure.Dto.VirtualMachine;
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

        /*
        public static async Task<AddDatasetToSandboxResult> OpenInternetVm(RestHelper restHelper, int sandboxId, int datasetId)
        {
            var response = await restHelper.Put<VmRuleDto>(String.Format(ApiUrls.SANDBOX_DATASETS, sandboxId, datasetId));

            return new AddDatasetToSandboxResult(response);
        }*/

        public static async Task<ApiConversation<VmRuleDto, TResponse>> OpenInternetForVm<TResponse>(RestHelper restHelper, string vmId = "1")
        {
            var request = new VmRuleDto() { Name = "OpenInternet", Action = 0, Description = "tests", Direction = 0, Ip = "1.1.1.1", Port = 80, Protocol = "HTTP" };
            var response = await restHelper.PostAsForm<TResponse, VmRuleDto>($"api/virtualmachines/{vmId}/rules", "study", request);

            return new ApiConversation<VmRuleDto, TResponse>(request, response);
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
