using Sepes.Common.Dto.VirtualMachine;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public class SandboxVirtualMachineRuleAsserts
    {
        public static void ExpectSuccess(VmRuleDto createRequest, VmRuleDto responseWrapper)
        {
            //ApiResponseBasicAsserts.ExpectSuccess<VmRuleDto>(responseWrapper);
            Assert.Equal(createRequest.Direction, responseWrapper.Direction);

        }

        public static void ExpectSuccess(ApiConversation<VmRuleDto, VmRuleDto> conversation)
        {
            ApiResponseBasicAsserts.ExpectSuccess<VmRuleDto>(conversation.Response);
            Assert.Equal(conversation.Request.Direction, conversation.Response.Content.Direction);

        }

        public static void ExpectSuccess(ApiConversation<VmRuleDto, List<VmRuleDto>> conversation)
        {
            var rule = conversation.Response.Content.FirstOrDefault();

            ExpectSuccess(conversation.Request, rule);
        }

        public static void ExpectSuccess(ApiConversation<List<VmRuleDto>, List<VmRuleDto>> conversation)
        {
            //var rule = conversation.Response.Content.FirstOrDefault();
            ApiResponseBasicAsserts.ExpectSuccess<List<VmRuleDto>>(conversation.Response);
            //ExpectSuccess(conversation.Request, rule);
        }
    }
}
