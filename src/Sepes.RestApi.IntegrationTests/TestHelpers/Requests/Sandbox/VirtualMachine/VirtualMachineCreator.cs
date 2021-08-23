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
        static async Task<ApiConversation<VirtualMachineCreateDto, TResponse>> Create<TResponse>(RestHelper restHelper, int sandboxId, string name = "integrationtest")
        {
            var request = CreateVmDtoFactory.New(name);
            var response = await restHelper.Post<VirtualMachineCreateDto, TResponse>(String.Format(ApiUrls.VIRTUAL_MACHINES, sandboxId), request);

            return new ApiConversation<VirtualMachineCreateDto, TResponse>(request, response);
        }


        public static async Task<ApiConversation<VirtualMachineCreateDto, VmDto>> CreateAndExpectSuccess(RestHelper restHelper, int sandboxId, string name = "integrationtest")
        {
            return await Create<VmDto>(restHelper, sandboxId, name);
        }

        public static async Task<ApiConversation<VirtualMachineCreateDto, Common.Response.ErrorResponse>> CreateAndExpectFailure(RestHelper restHelper, int sandboxId, string name = "integrationtest")
        {
            return await Create<Common.Response.ErrorResponse>(restHelper, sandboxId, name);
        }
    }   
}
