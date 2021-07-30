using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Service;
using System.Net.Http;

namespace Sepes.Azure.Service
{
    public class AzureApiServiceBase : RestApiServiceBase
    {
        public AzureApiServiceBase(IConfiguration config, ILogger logger, IAzureApiRequestAuthenticatorService azureApiRequestAuthenticatorService, HttpClient httpClient)
            : base(config, logger, azureApiRequestAuthenticatorService, httpClient, "https://management.azure.com/.default")
        {

        }        
    }
}
