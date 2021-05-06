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


        public static async Task<ApiConversation<VmRuleDto, TResponse>> OpenInternetForVm<TResponse>(RestHelper restHelper, string vmId = "1")
        {
            var request = new VmRuleDto() { Name = "OpenInternet", Action = RuleAction.Allow, Description = "tests", Direction = RuleDirection.Outbound, Ip = "1.1.1.1", Port = 80, Protocol = "HTTP" };
            var response = await restHelper.Post<TResponse, VmRuleDto>($"api/virtualmachines/{vmId}/rules", request);

            return new ApiConversation<VmRuleDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<VmRuleDto, TResponse>> CloseInternetForVm<TResponse>(RestHelper restHelper, string vmId = "1")
        {
            var request = new VmRuleDto() { Name = "OpenInternet", Action = RuleAction.Deny, Description = "tests", Direction = RuleDirection.Outbound, Ip = "1.1.1.1", Port = 80, Protocol = "HTTP" };
            var response = await restHelper.Post<TResponse, VmRuleDto>($"api/virtualmachines/{vmId}/rules", request);

            return new ApiConversation<VmRuleDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<SandboxDetails, TResponse>> MoveToNextPhase<TResponse>(RestHelper restHelper, string sandboxId = "1")
        {
            var request = new SandboxDetails() {  };
            var response = await restHelper.Post<TResponse, SandboxDetails>($"api/sandboxes/{sandboxId}/nextPhase", request);

            return new ApiConversation<SandboxDetails, TResponse>(request, response);
        }

        public static async void DeleteVm<TResponse>(RestHelper restHelper, string vmId = "1")
        {

            await restHelper.Delete<VmDto>($"api/virtualmachine/{vmId}");
            //return new ApiConversation<VmDto, TResponse>(request, response);
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
