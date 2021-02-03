using System.Net;

namespace Sepes.RestApi.IntegrationTests.Dto
{
    public class ApiResponseWrapper<T>
    {   
        public HttpStatusCode StatusCode { get; set; }
        public T Response { get; set; }       
    }
}
