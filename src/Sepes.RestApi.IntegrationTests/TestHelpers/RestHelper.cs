using Newtonsoft.Json;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.Extensions;
using System.Net.Http;
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

        public async Task<ApiResponseWrapper<T>> Post<T, K>(string requestUri, K request)
        {
            var response = await _client.PostAsJsonAsync(requestUri, request);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> PostAsForm<T, K>(string requestUri, string formkey, K request)
        {
            var response = await _client.PostAsFormAsync(requestUri, formkey, request);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> Post<T>(string requestUri)
        {
            var response = await _client.PostAsync(requestUri, null);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> Get<T>(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper> Get(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            var responseWrapper = CreateResponseWrapper(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> Delete<T>(string requestUri)
        {
            var response = await _client.DeleteAsync(requestUri);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> Put<T, K>(string requestUri, K request)
        {
            var response = await _client.PutAsJsonAsync(requestUri, request);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> PutAsForm<T, K>(string requestUri, string formKey, K request)
        {
            var response = await _client.PutAsFormAsync(requestUri, formKey, request);      
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        public async Task<ApiResponseWrapper<T>> Put<T>(string requestUri)
        {
            var response = await _client.PutAsync(requestUri, null);
            var responseWrapper = await CreateResponseWrapper<T>(response);
            return responseWrapper;
        }

        private async Task<T> GetResponseObject<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<T>(content);
            return deserializedObject;
        }

        async Task<ApiResponseWrapper<T>> CreateResponseWrapper<T>(HttpResponseMessage message)
        {
            var responseWrapper = new ApiResponseWrapper<T>
            {
                StatusCode = message.StatusCode,
                ReasonPhrase = message.ReasonPhrase,
                Content = await GetResponseObject<T>(message)
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
