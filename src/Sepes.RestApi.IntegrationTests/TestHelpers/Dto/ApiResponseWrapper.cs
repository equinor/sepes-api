using System.Net;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Dto
{

    public class ApiConversation<TResponse>
    {
        public ApiConversation(ApiResponseWrapper<TResponse> response)
        {           
            Response = response;
        }   

        public ApiResponseWrapper<TResponse> Response { get; private set; }
    }

    public class ApiConversation<TRequest, TResponse>
    {
        public ApiConversation(TRequest request, ApiResponseWrapper<TResponse> response)
        {
            Request = request;
            Response = response;
        }

        public TRequest Request { get; private set; }

        public ApiResponseWrapper<TResponse> Response { get; private set; }
    }

    public class ApiResponseWrapper
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ReasonPhrase { get; set; }
    }

    public class ApiResponseWrapper<T> : ApiResponseWrapper
    {
        public T Content { get; set; }       
    }  
}
