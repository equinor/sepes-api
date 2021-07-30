using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Service.Interface;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Common.Service
{
    public enum ApiTokenType
    {
        App,
        User
    }

    public class RestApiServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        protected readonly IHttpRequestAuthenticatorService _httpRequestAuthenticatorService;
        protected readonly HttpClient _httpClient;
        readonly string _scope;
        readonly ApiTokenType _apiTokenType;

        public RestApiServiceBase(IConfiguration config, ILogger logger, IHttpRequestAuthenticatorService httpRequestAuthenticatorService, HttpClient httpClient, string scope, ApiTokenType apiTokenType = ApiTokenType.App)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpRequestAuthenticatorService = httpRequestAuthenticatorService ?? throw new ArgumentNullException(nameof(httpRequestAuthenticatorService));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _scope = scope;
            _apiTokenType = apiTokenType;
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

        protected async Task<T> PerformRequest<T>(string url, HttpMethod method, HttpContent content = null, bool needsAuth = true, Dictionary<string, string> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            var requestMessage = new HttpRequestMessage(method, url);

            if (needsAuth)
            {
                if (_apiTokenType == ApiTokenType.App)
                {
                    await _httpRequestAuthenticatorService.PrepareRequestForAppAsync(requestMessage, _scope, cancellationToken);
                }
                else if (_apiTokenType == ApiTokenType.User)
                {
                    await _httpRequestAuthenticatorService.PrepareRequestForUserAsync(requestMessage, new List<string> { _scope }, cancellationToken);
                }
            }

            if (additionalHeaders != null)
            {
                foreach (var curHeader in additionalHeaders)
                {
                    requestMessage.Headers.Add(curHeader.Key, curHeader.Value);
                }
            }

            HttpResponseMessage responseMessage = null;

            if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch)
            {
                requestMessage.Content = content;
            }

            responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (responseMessage.IsSuccessStatusCode)
            {
                var responseText = await responseMessage.Content.ReadAsStringAsync();
                var deserializedResponse = JsonSerializerUtil.Deserialize<T>(responseText);
                return deserializedResponse;
            }
            else
            {
                var errorMessageBuilder = new StringBuilder();
                errorMessageBuilder.Append($"{this.GetType()}: Response for {method} against the url {url} failed with status code {responseMessage.StatusCode}");

                if (!String.IsNullOrWhiteSpace(responseMessage.ReasonPhrase))
                {
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