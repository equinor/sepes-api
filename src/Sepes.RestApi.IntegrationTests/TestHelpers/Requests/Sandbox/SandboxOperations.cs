using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class SandboxOperations
    {      
        //INBOUND RULES
        public static async Task<ApiConversation<List<VmRuleDto>, List<VmRuleDto>>> AddVmInboundRuleExpectSuccess(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await AddVmInboundRuleInternal<List<VmRuleDto>>(restHelper, vmId, existingRules);
        }

        public static async Task<ApiConversation<List<VmRuleDto>, Common.Response.ErrorResponse>> AddVmInboundRuleExpectFailure(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await AddVmInboundRuleInternal<Common.Response.ErrorResponse>(restHelper, vmId, existingRules);
        }

        static async Task<ApiConversation<List<VmRuleDto>, TResponse>> AddVmInboundRuleInternal<TResponse>(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            var newRule = new VmRuleDto() { Name = "SomeRule", Action = RuleAction.Allow, Description = "tests", Direction = RuleDirection.Inbound, Ip = "1.1.1.1", Port = 80, Protocol = "HTTP" };
            existingRules.Add(newRule);            
            return await PostVmRulesInternal<TResponse>(restHelper, vmId, existingRules);
        }

        //OPEN INTERNET      

        public static async Task<ApiConversation<List<VmRuleDto>, List<VmRuleDto>>> OpenInternetForVmExpectSuccess(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await OpenInternetForVmInternal<List<VmRuleDto>>(restHelper, vmId, existingRules);
        }

        public static async Task<ApiConversation<List<VmRuleDto>, Common.Response.ErrorResponse>> OpenInternetForVmExpectFailure(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await OpenInternetForVmInternal<Common.Response.ErrorResponse>(restHelper, vmId, existingRules);
        }
        
        static async Task<ApiConversation<List<VmRuleDto>, TResponse>> OpenInternetForVmInternal<TResponse>(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await SetInternetAllowedInternal<TResponse>(restHelper, vmId, existingRules, RuleAction.Allow);
        }      

        //CLOSE INTERNET
        public static async Task<ApiConversation<List<VmRuleDto>, List<VmRuleDto>>> CloseInternetForVmExpectSuccess(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await CloseInternetForVmInternal<List<VmRuleDto>>(restHelper, vmId, existingRules);
        }

        public static async Task<ApiConversation<List<VmRuleDto>, Common.Response.ErrorResponse>> CloseInternetForVmExpectFailure(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await CloseInternetForVmInternal<Common.Response.ErrorResponse>(restHelper, vmId, existingRules);
        }

        static async Task<ApiConversation<List<VmRuleDto>, TResponse>> CloseInternetForVmInternal<TResponse>(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules)
        {
            return await SetInternetAllowedInternal<TResponse>(restHelper, vmId, existingRules, RuleAction.Deny);
        }

        //COMMON
        static async Task<ApiConversation<List<VmRuleDto>, TResponse>> SetInternetAllowedInternal<TResponse>(RestHelper restHelper, int vmId, List<VmRuleDto> existingRules, RuleAction ruleAction)
        {
            var existingInternetRule = existingRules.Where(r => r.Name.Contains(AzureVmConstants.RulePresets.OPEN_CLOSE_INTERNET)).SingleOrDefault();
            existingInternetRule.Action = ruleAction;
            var requestResult = await PostVmRulesInternal<TResponse>(restHelper, vmId, existingRules);
            return requestResult;
        }

        static async Task<ApiConversation<VmRuleDto, TResponse>> AddVmRuleInternal<TResponse>(RestHelper restHelper, int vmId, VmRuleDto newRule)
        {
            var response = await restHelper.Post<List<VmRuleDto>, TResponse>($"api/virtualmachines/{vmId}/rules", CreateRequest(newRule));
            return new ApiConversation<VmRuleDto, TResponse>(newRule, response);
        }

        static async Task<ApiConversation<List<VmRuleDto>, TResponse>> PostVmRulesInternal<TResponse>(RestHelper restHelper, int vmId, List<VmRuleDto> rules)
        {
            var response = await restHelper.Post<List<VmRuleDto>, TResponse>($"api/virtualmachines/{vmId}/rules", rules);
            return new ApiConversation<List<VmRuleDto>, TResponse>(rules, response);
        }

        static List<VmRuleDto> CreateRequest(VmRuleDto newRule, List<VmRuleDto> existingRules = null)
        {
            var request = existingRules != null ? existingRules : new List<VmRuleDto>();
            request.Add(newRule);
            return request;
        }

        //NEXT PHASE

        public static async Task<ApiConversation<SandboxDetails, TResponse>> MoveToNextPhase<TResponse>(RestHelper restHelper, int sandboxId)
        {
            var request = new SandboxDetails() {  };
            var response = await restHelper.Post<SandboxDetails, TResponse>($"api/sandboxes/{sandboxId}/nextPhase", request);

            return new ApiConversation<SandboxDetails, TResponse>(request, response);
        }

        public static async Task<ApiConversation<NoContentResult>> DeleteVm(RestHelper restHelper, int vmId)
        {            
            return await GenericDeleter.DeleteAndExpectSuccess(restHelper, $"api/virtualmachines/{vmId}");             
        }
    } 
}
