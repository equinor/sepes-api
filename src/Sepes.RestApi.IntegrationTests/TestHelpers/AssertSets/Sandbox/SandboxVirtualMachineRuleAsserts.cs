using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.RestApi.IntegrationTests.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}
