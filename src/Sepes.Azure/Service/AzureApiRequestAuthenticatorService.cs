using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureApiRequestAuthenticatorService : IAzureApiRequestAuthenticatorService
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _config;
        protected readonly IAzureCredentialService _azureCredentialService; 

        public AzureApiRequestAuthenticatorService(IConfiguration config, ILogger<AzureApiRequestAuthenticatorService> logger, IAzureCredentialService azureCredentialService)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _azureCredentialService = azureCredentialService ?? throw new ArgumentNullException(nameof(azureCredentialService));      
        }     

        public async Task PrepareRequestForAppAsync(HttpRequestMessage httpRequestMessage, string scope, CancellationToken cancellationToken = default)
        {
            var credential = _azureCredentialService.GetAzureCredentials();
            await ApplyCredentialToRequest(httpRequestMessage, credential, cancellationToken);
        }

        public async Task PrepareRequestForUserAsync(HttpRequestMessage httpRequestMessage, IEnumerable<string> scopes, CancellationToken cancellationToken = default)
        {
            var credential = _azureCredentialService.GetAzureCredentials();
            await ApplyCredentialToRequest(httpRequestMessage, credential, cancellationToken);
        }

        async Task ApplyCredentialToRequest(HttpRequestMessage httpRequestMessage, AzureCredentials azureCredentials, CancellationToken cancellationToken = default)
        {
            await azureCredentials.ProcessHttpRequestAsync(httpRequestMessage, cancellationToken);
        }
    }
}