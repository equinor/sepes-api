using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class SandboxOperations
    { 
        public static async Task<ApiConversation<VmRuleDto, TResponse>> OpenInternetForVm<TResponse>(RestHelper restHelper, int vmId)
        {
            var request = new VmRuleDto() { Name = "OpenInternet", Action = RuleAction.Allow, Description = "tests", Direction = RuleDirection.Outbound, Ip = "1.1.1.1", Port = 80, Protocol = "HTTP" };
            var response = await restHelper.Post<TResponse, VmRuleDto>($"api/virtualmachines/{vmId}/rules", request);

            return new ApiConversation<VmRuleDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<VmRuleDto, TResponse>> CloseInternetForVm<TResponse>(RestHelper restHelper, int vmId)
        {
            var request = new VmRuleDto() { Name = "OpenInternet", Action = RuleAction.Deny, Description = "tests", Direction = RuleDirection.Outbound, Ip = "1.1.1.1", Port = 80, Protocol = "HTTP" };
            var response = await restHelper.Post<TResponse, VmRuleDto>($"api/virtualmachines/{vmId}/rules", request);

            return new ApiConversation<VmRuleDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<SandboxDetails, TResponse>> MoveToNextPhase<TResponse>(RestHelper restHelper, int sandboxId)
        {
            var request = new SandboxDetails() {  };
            var response = await restHelper.Post<TResponse, SandboxDetails>($"api/sandboxes/{sandboxId}/nextPhase", request);

            return new ApiConversation<SandboxDetails, TResponse>(request, response);
        }

        public static async Task<ApiConversation<NoContentResult>> DeleteVm(RestHelper restHelper, int vmId)
        {            
            return await GenericDeleter.DeleteAndExpectSuccess(restHelper, $"api/virtualmachines/{vmId}");             
        }
    } 
}
