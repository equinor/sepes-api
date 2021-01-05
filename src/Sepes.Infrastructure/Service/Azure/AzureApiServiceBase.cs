﻿using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Model.Config;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureApiServiceBase
    {
        protected string[] scopes = new string[] { "https://management.azure.com/.default" };

        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        protected readonly AzureCredentials _credentials;
        protected readonly ITokenAcquisition _tokenAcquisition;

        readonly string _tokenUrl;
        readonly string _tenantId;
        readonly string _clientId;
        readonly string _clientSecret;
        readonly string _instance;

        protected readonly string _subscriptionId;


        public AzureApiServiceBase(IConfiguration config, ILogger logger, ITokenAcquisition tokenAcquisition)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenAcquisition = tokenAcquisition;
            _tenantId = config[ConfigConstants.AZ_TENANT_ID];
            _clientId = config[ConfigConstants.AZ_CLIENT_ID];
            _clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];
            _instance = config[ConfigConstants.AZ_INSTANCE];

            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];

            _tokenUrl = $"{_instance}{_tenantId}/oauth2/token";
        }

        protected void SetScopes(string[] newScopes)
        {
            scopes = newScopes;
        }

        protected async Task<string> AquireTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {

                using (var tokenClient = new HttpClient())
                {
                    var tokenRequestBodyObject = new { grant_type = "client_credentials", client_id = _clientId, client_secret = _clientSecret, resource = "https://management.azure.com/" };
                    var tokenRequestBodyJson = new StringContent(JsonConvert.SerializeObject(tokenRequestBodyObject), Encoding.UTF8, "application/json");

                    var tokenResponseMessage = await tokenClient.PostAsync(_tokenUrl, tokenRequestBodyJson, cancellationToken);

                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await tokenResponseMessage.Content.ReadAsStringAsync());

                    if (String.IsNullOrWhiteSpace(tokenResponse.access_token))
                    {
                        throw new Exception("Token was null or empty");
                    }

                    return tokenResponse.access_token;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.GetType()}: Aquire token failed", ex);
            }
        }

        protected async Task<T> GetResponse<T>(string url, bool needsAuth = true, CancellationToken cancellationToken = default)
        {
            try
            {
                return await PerformRequest<T>(url, HttpMethod.Get, content: null, needsAuth, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.GetType()}: GetResponse for the url {url} failed", ex);
            }
        }

        protected async Task<T> PerformRequest<T>(string url, HttpMethod method, HttpContent content = null, bool needsAuth = true, CancellationToken cancellationToken = default)
        {
            try
            {
                string token = null;

                if (needsAuth)
                {                   
                    string[] scopes = new string[] { "https://management.azure.com/.default" };
                    token = await _tokenAcquisition.GetAccessTokenForAppAsync(scopes);
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
                    else if(method == HttpMethod.Post)
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

                    var responseString = await responseMessage.Content.ReadAsStringAsync();

                    var deserializedResponse = JsonConvert.DeserializeObject<T>(await responseMessage.Content.ReadAsStringAsync());

                    return deserializedResponse;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.GetType()}: Response for {method} against the url {url} failed", ex);
            }
        }
    }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    class TokenResponse
    {
       
        public string access_token { get; set; }

        public string token_type { get; set; }

        public string expires_in { get; set; }

        public string expires_on { get; set; }

        public string resource { get; set; }
    }
}