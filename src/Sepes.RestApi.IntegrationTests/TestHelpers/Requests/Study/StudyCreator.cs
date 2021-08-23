using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudyCreator
    {
        static async Task<ApiConversation<StudyCreateDto, TResponse>> Create<TResponse>(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")         
        {
            var request = new StudyCreateDto() { Name = studyName, Vendor = vendor, WbsCode = wbsCode };
            var response = await restHelper.PostAsForm<StudyCreateDto, TResponse>("api/studies", "study", request);

            return new ApiConversation<StudyCreateDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<StudyCreateDto, StudyDetailsDto>> CreateAndExpectSuccess(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")
        {
            return await Create<StudyDetailsDto>(restHelper, studyName, vendor, wbsCode);
        }

        public static async Task<ApiConversation<StudyCreateDto, Common.Response.ErrorResponse>> CreateAndExpectFailure(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")
        {
            return await Create<Common.Response.ErrorResponse>(restHelper, studyName, vendor, wbsCode);
        }
    } 
}
