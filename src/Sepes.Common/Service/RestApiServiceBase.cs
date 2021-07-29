﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
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
        protected readonly ITokenAcquisition _tokenAcquisition;
        protected readonly HttpClient _httpClient;
        readonly string _scope;
        readonly ApiTokenType _apiTokenType;

        public RestApiServiceBase(IConfiguration config, ILogger logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient, string scope, ApiTokenType apiTokenType = ApiTokenType.App)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            //_tokenAcquisition = tokenAcquisition ?? throw new ArgumentNullException(nameof(tokenAcquisition));
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
            if (needsAuth)
            {
                var token = _apiTokenType == ApiTokenType.App ? await _tokenAcquisition.GetAccessTokenForAppAsync(_scope) : await _tokenAcquisition.GetAccessTokenForUserAsync(scopes: new List<string>() { _scope });
               var _credentials = new AzureCredentials
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if(additionalHeaders != null)
            {
                foreach(var curHeader in additionalHeaders)
                {
                    _httpClient.DefaultRequestHeaders.Add(curHeader.Key, curHeader.Value);
                }
            }

            HttpResponseMessage responseMessage = null;

            if (method == HttpMethod.Get)
            {
                responseMessage = await _httpClient.GetAsync(url, cancellationToken);
            }
            else if (method == HttpMethod.Post)
            {
                responseMessage = await _httpClient.PostAsync(url, content, cancellationToken);
            }
            else if (method == HttpMethod.Put)
            {
                responseMessage = await _httpClient.PutAsync(url, content, cancellationToken);
            }
            else if (method == HttpMethod.Patch)
            {
                responseMessage = await _httpClient.PatchAsync(url, content, cancellationToken);
            }
            else if (method == HttpMethod.Delete)
            {
                responseMessage = await _httpClient.DeleteAsync(url, cancellationToken);
            }

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