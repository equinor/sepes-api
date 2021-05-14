using Sepes.Common.Dto.Dataset;
using Sepes.RestApi.IntegrationTests.Constants;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class DatasetCreator
    {
        public static async Task<DatasetSeedResult> Create(RestHelper restHelper, int studyId, string location = "norwayeast", string name = "datasetName", string classification = "open")         
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = name, Location = location, Classification = classification };
            var response = await restHelper.Post<DatasetDto, DatasetCreateUpdateInputBaseDto>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS, studyId), request);

            return new DatasetSeedResult(request, response);
        }
    }

    public class DatasetSeedResult
    {
        public DatasetSeedResult(DatasetCreateUpdateInputBaseDto request, ApiResponseWrapper<DatasetDto> response)
        {
            Request = request;
            Response = response;
        }

        public DatasetCreateUpdateInputBaseDto Request { get; private set; }

        public ApiResponseWrapper<DatasetDto> Response { get; private set; }       
    }
}
