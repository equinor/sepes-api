using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class StudyUpdater
    {
        static async Task<ApiConversation<StudyCreateDto, TResponse>> Update<TResponse>(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")         
        {
            var request = new StudyCreateDto() { Name = studyName, Vendor = vendor, WbsCode = wbsCode };
            var response = await restHelper.Post<TResponse, StudyCreateDto>("api/studies", request);

            return new ApiConversation<StudyCreateDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<StudyCreateDto, StudyDto>> UpdateAndExpectSuccess(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")
        {
            return await Update<StudyDto>(restHelper, studyName, vendor, wbsCode);
        }

        public static async Task<ApiConversation<StudyCreateDto, Infrastructure.Dto.ErrorResponse>> UpdateAndExpectFailure(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")
        {
            return await Update<Infrastructure.Dto.ErrorResponse>(restHelper, studyName, vendor, wbsCode);
        }
    } 
}
