using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Common.Service
{
    public class RestApiServiceBase
    {        
        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        protected readonly ITokenAcquisition _tokenAcquisition;           

        public RestApiServiceBase(IConfiguration config, ILogger logger, ITokenAcquisition tokenAcquisition)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenAcquisition = tokenAcquisition;
                    
        }             

        protected async Task<T> GetResponse<T>(string url, bool needsAuth = true, CancellationToken cancellationToken = default)
        {
            try
            {
                return await PerformRequest<T>(url, HttpMethod.Get, content: null, needsAuth: needsAuth, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.GetType()}: GetResponse for the url {url} failed", ex);
            }
        }

        protected async Task<T> PerformRequest<T>(string url, HttpMethod method, HttpContent content = null, bool needsAuth = true, CancellationToken cancellationToken = default)
        {

            string token = null;

            if (needsAuth)
            {
                token = await _tokenAcquisition.GetAccessTokenForAppAsync("https://management.azure.com/.default");
            }

            using (var apiRequestClient = new HttpClient())
            {
                if (needsAuth)
                {
                    apiRequestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage responseMessage = null;

                if (method == HttpMethod.Get)
                {
                    responseMessage = await apiRequestClient.GetAsync(url, cancellationToken);
                }
                else if (method == HttpMethod.Post)
                {
                    responseMessage = await apiRequestClient.PostAsync(url, content, cancellationToken);
                }
                else if (method == HttpMethod.Put)
                {
                    responseMessage = await apiRequestClient.PutAsync(url, content, cancellationToken);
                }
                else if (method == HttpMethod.Patch)
                {
                    responseMessage = await apiRequestClient.PatchAsync(url, content, cancellationToken);
                }
                else if (method == HttpMethod.Delete)
                {
                    responseMessage = await apiRequestClient.DeleteAsync(url, cancellationToken);
                }

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseString = await responseMessage.Content.ReadAsStringAsync();
                    var deserializedResponse = JsonConvert.DeserializeObject<T>(await responseMessage.Content.ReadAsStringAsync());
                    return deserializedResponse;
                }
                else
                {
                    var errorMessageBuilder = new StringBuilder();
                    errorMessageBuilder.Append($"{this.GetType()}: Response for {method} against the url {url} failed with status code {responseMessage.StatusCode}");

                    if (!String.IsNullOrWhiteSpace(responseMessage.ReasonPhrase)) {
                        errorMessageBuilder.Append($", reason: {responseMessage.ReasonPhrase}");
                    }

                    var responseString = await responseMessage.Content.ReadAsStringAsync();

                    if (!String.IsNullOrWhiteSpace(responseString))
                    {
                        errorMessageBuilder.Append($", response content: {responseString}");
                    }
                   
                    throw new Exception(errorMessageBuilder.ToString());
                }
            }
        }
    } 
}