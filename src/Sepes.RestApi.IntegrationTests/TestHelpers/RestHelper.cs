using Sepes.Common.Util;
using Sepes.RestApi.IntegrationTests.Setup.Extensions;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers
{
    public class RestHelper
    {
        private readonly HttpClient _client;

        public RestHelper(HttpClient client)
        {
            _client = client;
        }       

        public async Task<ApiResponseWrapper<TResponse>> Post<TRequest, TResponse>(string requestUri, TRequest request)
        {
            var response = await _client.PostAsJsonAsync(requestUri, request);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> PostAsForm<TRequest, TResponse>(string requestUri, string formkey, TRequest request)
        {
            var response = await _client.PostAsFormAsync(requestUri, formkey, request);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> Post<TResponse>(string requestUri)
        {
            var response = await _client.PostAsync(requestUri, null);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> Get<TResponse>(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper> Get(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            var responseWrapper = CreateResponseWrapper(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> Delete<TResponse>(string requestUri)
        {
            var response = await _client.DeleteAsync(requestUri);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> Put<TRequest, TResponse>(string requestUri, TRequest request)
        {
            var response = await _client.PutAsJsonAsync(requestUri, request);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> PutAsForm<TRequest, TResponse>(string requestUri, string formKey, TRequest request)
        {
            var response = await _client.PutAsFormAsync(requestUri, formKey, request);      
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<TResponse>> Put<TResponse>(string requestUri)
        {
            var stringContent = new StringContent(string.Empty);
            stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            var response = await _client.PutAsJsonAsync(requestUri, stringContent);
            var responseWrapper = await CreateResponseWrapper<TResponse>(response);
            return responseWrapper;
        }

        async Task<TResponse> GetResponseObject<TResponse>(HttpResponseMessage response)
        {
            if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return default(TResponse);
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return default(TResponse);
            }

            var deserializedObject = JsonSerializerUtil.Deserialize<TResponse>(content);
            return deserializedObject;
        }

        async Task<ApiResponseWrapper<TResponse>> CreateResponseWrapper<TResponse>(HttpResponseMessage message)
        {
            var responseWrapper = new ApiResponseWrapper<TResponse>
            {
                StatusCode = message.StatusCode,
                ReasonPhrase = message.ReasonPhrase,
                Content = await GetResponseObject<TResponse>(message)
            };

            return responseWrapper;
        }

        ApiResponseWrapper CreateResponseWrapper(HttpResponseMessage message)
        {
            var responseWrapper = new ApiResponseWrapper
            {
                StatusCode = message.StatusCode
            };
            return responseWrapper;
        }
    }
}
