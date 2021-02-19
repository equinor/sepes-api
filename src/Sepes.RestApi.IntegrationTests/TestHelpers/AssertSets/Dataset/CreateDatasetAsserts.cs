using Sepes.Infrastructure.Dto.Dataset;
using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset
{
    public static class CreateDatasetAsserts
    {
        public static void ExpectSuccess(DatasetCreateUpdateInputBaseDto createRequest, ApiResponseWrapper<DatasetDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<DatasetDto>(responseWrapper);    
            Assert.NotEqual<int>(0, responseWrapper.Response.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Response.Name);
            Assert.Equal(createRequest.Classification, responseWrapper.Response.Classification);
        }
    }
}
