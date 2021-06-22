using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Net;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class DeleteBasicAsserts
    {
        public static void ExpectSuccess(ApiResponseWrapper<NoContentResult> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess(responseWrapper);          
        }

        public static void ExpectNoContent(ApiResponseWrapper<NoContentResult> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectNoContent(responseWrapper);
        }

        public static void ExpectFailureWithMessage(ApiResponseWrapper<Common.Dto.ErrorResponse> responseWrapper, HttpStatusCode statusCode, string messageShouldContain = null)
        {
            ApiResponseBasicAsserts.ExpectFailureWithMessage(responseWrapper, statusCode, messageShouldContain);
        }

        public static void ExpectForbiddenWithMessage(ApiResponseWrapper<Common.Dto.ErrorResponse> responseWrapper, string messageShouldContain = null)
        {
            ExpectFailureWithMessage(responseWrapper, HttpStatusCode.Forbidden, messageShouldContain);
        }
    }
}
