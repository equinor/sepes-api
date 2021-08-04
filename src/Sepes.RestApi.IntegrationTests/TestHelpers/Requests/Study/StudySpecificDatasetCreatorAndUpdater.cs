using Sepes.Common.Dto.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudySpecificDatasetCreatorAndUpdater
    {
        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, DatasetDto>> CreateExpectSuccess(RestHelper restHelper, int studyId, string location = "norwayeast", string name = "datasetName", string classification = "open")         
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = name, Location = location, Classification = classification };
            var response = await restHelper.Post<DatasetDto, DatasetCreateUpdateInputBaseDto>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS, studyId), request);

            return new ApiConversation<DatasetCreateUpdateInputBaseDto, DatasetDto>(request, response);
        }

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> CreateExpectFailure(RestHelper restHelper, int studyId, string location = "norwayeast", string name = "datasetName", string classification = "open")
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = name, Location = location, Classification = classification };
            var response = await restHelper.Post<Common.Dto.ErrorResponse, DatasetCreateUpdateInputBaseDto>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS, studyId), request);

            return new ApiConversation<Common.Dto.ErrorResponse>(response);
        }


        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, DatasetDto>> UpdateExpectSuccess(RestHelper restHelper, int studyId, int datasetId, string newName = "newDatasetName", string newClassification = "restricted")
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = newName, Classification = newClassification, Location = "norwayeast" };
            var response = await restHelper.Put<DatasetDto, DatasetCreateUpdateInputBaseDto>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS_UPDATE, studyId, datasetId), request);

            return new ApiConversation<DatasetCreateUpdateInputBaseDto, DatasetDto>(request, response);
        }

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> UpdateExpectFailure(RestHelper restHelper, int studyId, int datasetId, string newName = "newDatasetName", string newClassification = "restricted")
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = newName, Classification = newClassification, Location = "norwayeast" };
            var response = await restHelper.Put<Common.Dto.ErrorResponse, DatasetCreateUpdateInputBaseDto>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS_UPDATE, studyId, datasetId), request);

            return new ApiConversation<Common.Dto.ErrorResponse>(response);
        }
    }  
}
