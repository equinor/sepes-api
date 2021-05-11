using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Constants;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class SandboxCreator
    {
        static async Task<ApiConversation<SandboxCreateDto, TResponse>> Create<TResponse>(RestHelper restHelper, int studyId, string sandboxName = "sandboxName", string region = "norwayeast")         
        {
            var request = new SandboxCreateDto() {  Name = sandboxName, Region = region };
            var response = await restHelper.Post<TResponse, SandboxCreateDto>(String.Format(ApiUrls.STUDY_SANDBOXES, studyId), request);

            return new ApiConversation<SandboxCreateDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<SandboxCreateDto, SandboxDetails>> CreateAndExpectSuccess(RestHelper restHelper, int studyId, string sandboxName = "sandboxName", string region = "norwayeast")
        {
            return await Create<SandboxDetails>(restHelper, studyId, sandboxName, region);
        }

        public static async Task<ApiConversation<SandboxCreateDto, Infrastructure.Dto.ErrorResponse>> CreateAndExpectFailure(RestHelper restHelper, int studyId, string sandboxName = "sandboxName", string region = "norwayeast")
        {
            return await Create<Infrastructure.Dto.ErrorResponse>(restHelper, studyId, sandboxName, region);
        }
    } 
}
