using Sepes.Common.Dto.VirtualMachine;
using Sepes.RestApi.IntegrationTests.TestHelpers.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Sepes.Tests.Common.ModelFactory.VirtualMachine;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class VirtualMachineCreator
    {
        public static async Task<VirtualMachineSeedResult> Create(RestHelper restHelper, int sandboxId, string name = "integrationtest")         
        {
            var request = CreateVmDtoFactory.New(name);
            var response = await restHelper.Post<VmDto, VirtualMachineCreateDto>(String.Format(ApiUrls.VIRTUAL_MACHINES, sandboxId), request);

            return new VirtualMachineSeedResult(request, response);
        }
    }

    public class VirtualMachineSeedResult
    {
        public VirtualMachineSeedResult(VirtualMachineCreateDto request, ApiResponseWrapper<VmDto> response)
        {
            Request = request;
            Response = response;
        }

        public VirtualMachineCreateDto Request { get; private set; }

        public ApiResponseWrapper<VmDto> Response { get; private set; }       
    }
}
