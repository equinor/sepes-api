using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class StudyCreator
    {
        public static async Task<StudyCreateResult> Create(RestHelper restHelper, string studyName = "studyName", string vendor = "Vendor", string wbsCode = "wbs")         
        {
            var request = new StudyCreateDto() { Name = studyName, Vendor = vendor, WbsCode = wbsCode };
            var response = await restHelper.Post<StudyDto, StudyCreateDto>("api/studies", request);

            return new StudyCreateResult(request, response);
        }
    }

    public class StudyCreateResult
    {
        public StudyCreateResult(StudyCreateDto request, ApiResponseWrapper<StudyDto> response)
        {
            Request = request;
            Response = response;
        }

        public StudyCreateDto Request { get; private set; }

        public ApiResponseWrapper<StudyDto> Response { get; private set; }       
    }
}
