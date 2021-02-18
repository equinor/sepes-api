using Sepes.Infrastructure.Dto.Sandbox;
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
        public static async Task<SandboxSeedResult> Create(RestHelper restHelper, int studyId, string name = "sandboxName", string region = "norwayeast")         
        {
            var request = new SandboxCreateDto() { Name = name, Region = region };
            var response = await restHelper.Post<SandboxDetails, SandboxCreateDto>(String.Format(ApiUrls.SANDBOXES, studyId), request);

            return new SandboxSeedResult(request, response);
        }
    }

    public class SandboxSeedResult
    {
        public SandboxSeedResult(SandboxCreateDto request, ApiResponseWrapper<SandboxDetails> response)
        {
            Request = request;
            Response = response;
        }

        public SandboxCreateDto Request { get; private set; }

        public ApiResponseWrapper<SandboxDetails> Response { get; private set; }       
    }
}
