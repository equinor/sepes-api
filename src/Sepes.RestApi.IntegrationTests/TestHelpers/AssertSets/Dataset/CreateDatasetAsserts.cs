using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset
{
    public static class CreateDatasetAsserts
    {
        public static void ExpectSuccess(DatasetCreateUpdateInputBaseDto createRequest, ApiResponseWrapper<DatasetDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<DatasetDto>(responseWrapper);    
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Content.Name);
            Assert.Equal(createRequest.Classification, responseWrapper.Content.Classification);
        }

        public static void ExpectDeleteSuccess(ApiResponseWrapper<NoContentResult> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectNoContent(responseWrapper);         
        }
    }
}
