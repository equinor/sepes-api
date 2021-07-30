using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Service.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Common.Service
{
    public class RequestAuthenticatorWithTokenAquistionService : IRequestAuthenticatorWithTokenAquistionService
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        protected readonly IRestApiTokenAquisitionWithIdentityService _restApiTokenAquisitionService; 

        public RequestAuthenticatorWithTokenAquistionService(IConfiguration config, ILogger<RestApiTokenAquisitionWithIdentityService> logger, IRestApiTokenAquisitionWithIdentityService restApiTokenAquisitionService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _restApiTokenAquisitionService = restApiTokenAquisitionService ?? throw new ArgumentNullException(nameof(restApiTokenAquisitionService));      
        }     

        public async Task PrepareRequestForAppAsync(HttpRequestMessage httpRequestMessage, string scope, CancellationToken cancellationToken = default)
        {
            var token = await _restApiTokenAquisitionService.GetAccessTokenForAppAsync(scope);
            ApplyAccessTokenToRequest(httpRequestMessage, token);
        }

        public async Task PrepareRequestForUserAsync(HttpRequestMessage httpRequestMessage, IEnumerable<string> scopes, CancellationToken cancellationToken = default)
        {
            var token = await _restApiTokenAquisitionService.GetAccessTokenForUserAsync(scopes: scopes);
            ApplyAccessTokenToRequest(httpRequestMessage, token);
        }

        void ApplyAccessTokenToRequest(HttpRequestMessage httpRequestMessage, string token)
        {
            httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}