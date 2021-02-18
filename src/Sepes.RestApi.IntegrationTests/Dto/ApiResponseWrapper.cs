using System.Net;

namespace Sepes.RestApi.IntegrationTests.Dto
{
    public class ApiResponseWrapper<T> : ApiResponseWrapper
    {
        public T Response { get; set; }       
    }

    public class ApiResponseWrapper
    {
        public HttpStatusCode StatusCode { get; set; }      
    }
}
