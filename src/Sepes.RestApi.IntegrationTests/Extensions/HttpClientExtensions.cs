using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Extensions
{
    public static class HttpClientExtensions
    {      
        public static async Task<HttpResponseMessage> PostAsFormAsync<TValue>(this HttpClient client, string requestUri, string formKey, TValue value, CancellationToken cancellationToken = default)
        {
           return await client.PostAsync(requestUri, CreateContent<TValue>(formKey, value), cancellationToken: cancellationToken);       
        }

        public static async Task<HttpResponseMessage> PutAsFormAsync<TValue>(this HttpClient client, string requestUri, string formKey, TValue value, CancellationToken cancellationToken = default)
        {
            return await client.PutAsync(requestUri, CreateContent<TValue>(formKey, value), cancellationToken: cancellationToken);
        }

        static MultipartFormDataContent CreateContent<TValue>(string formKey, TValue value)
        {
            var valueAsJson = JsonSerializer.Serialize(value);

            var formContent = new MultipartFormDataContent("integrationtestboundrary")
            {
                { new StringContent(valueAsJson, Encoding.UTF8), formKey }
            };

            return formContent;
        }
    }
}
