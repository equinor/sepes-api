﻿using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudySpecificDatasetCreateUpdateDelete
    {
        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, DatasetDto>> CreateExpectSuccess(RestHelper restHelper, int studyId, string location = "norwayeast", string name = "datasetName", string classification = "open")         
        {
            return await Create<DatasetDto>(restHelper, studyId, location, name, classification);      
        }

        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, Common.Response.ErrorResponse>> CreateExpectFailure(RestHelper restHelper, int studyId, string location = "norwayeast", string name = "datasetName", string classification = "open")
        {
            return await Create<Common.Response.ErrorResponse>(restHelper, studyId, location, name, classification);       
        }

        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, TResponse>> Create<TResponse>(RestHelper restHelper, int studyId, string location = "norwayeast", string name = "datasetName", string classification = "open")
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = name, Location = location, Classification = classification };
            var response = await restHelper.Post<DatasetCreateUpdateInputBaseDto, TResponse>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS, studyId), request);

            return new ApiConversation<DatasetCreateUpdateInputBaseDto, TResponse>(request, response);
        }


        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, DatasetDto>> UpdateExpectSuccess(RestHelper restHelper, int studyId, int datasetId, string newName = "newDatasetName", string newClassification = "restricted")
        {
            return await Update<DatasetDto>(restHelper, studyId, datasetId, newName, newClassification);
        }

        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, Common.Response.ErrorResponse>> UpdateExpectFailure(RestHelper restHelper, int studyId, int datasetId, string newName = "newDatasetName", string newClassification = "restricted")
        {
            return await Update<Common.Response.ErrorResponse>(restHelper, studyId, datasetId, newName, newClassification);
        }

        public static async Task<ApiConversation<DatasetCreateUpdateInputBaseDto, TResponse>> Update<TResponse>(RestHelper restHelper, int studyId, int datasetId, string newName = "newDatasetName", string newClassification = "restricted")
        {
            var request = new DatasetCreateUpdateInputBaseDto() { Name = newName, Classification = newClassification, Location = "norwayeast" };
            var response = await restHelper.Put<DatasetCreateUpdateInputBaseDto, TResponse>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASETS_UPDATE, studyId, datasetId), request);

            return new ApiConversation<DatasetCreateUpdateInputBaseDto, TResponse>(request, response);
        }

        public static async Task<ApiConversation<NoContentResult>> DeleteExpectSuccess(RestHelper restHelper, int datasetId)
        {
            var response = await restHelper.Delete<NoContentResult>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASET_DELETE, datasetId));

            return new ApiConversation<NoContentResult>(response);
        }

        public static async Task<ApiConversation<Common.Response.ErrorResponse>> DeleteExpectFailure(RestHelper restHelper, int datasetId)
        {
            var response = await restHelper.Delete<Common.Response.ErrorResponse>(String.Format(ApiUrls.STUDY_SPECIFIC_DATASET_DELETE, datasetId));

            return new ApiConversation<Common.Response.ErrorResponse>(response);
        }


        public static async Task<ApiConversation<T>> Delete<T>(RestHelper restHelper, int datasetId)
        {          
            var response = await restHelper.Delete<T>(String.Format(ApiUrls.DATASET, datasetId));

            return new ApiConversation<T>(response);
        }        
    }  
}
