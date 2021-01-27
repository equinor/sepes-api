using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sepes.API.IntegrationTests.TestHelpers
{
    public class RestHelper
    {
        private readonly HttpClient _client;

        public RestHelper(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> Post<T, K>(string requestUri, K request)
        {
            var response = await _client.PostAsJsonAsync(requestUri, request);
            T deserializedObject = await GetResponseObject<T>(response);
            return deserializedObject;
        }

        public async Task<T> Get<T>(string requestUri)
        {
            var response = await _client.GetAsync(requestUri);
            T deserializedObject = await GetResponseObject<T>(response);
            return deserializedObject;
        }

        public async Task<T> Delete<T>(string requestUri)
        {
            var response = await _client.DeleteAsync(requestUri);
            T deserializedObject = await GetResponseObject<T>(response);
            return deserializedObject;
        }

        public async Task<T> Put<T, K>(string requestUri, K request)
        {
            var response = await _client.PutAsJsonAsync(requestUri, request);
            T deserializedObject = await GetResponseObject<T>(response);
            return deserializedObject;
        }

        private async Task<T> GetResponseObject<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<T>(content);
            return deserializedObject;
        }
    }
}
