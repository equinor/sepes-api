using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Service;
using System.Net.Http;

namespace Sepes.Azure.Service
{
    public class AzureApiServiceBase : RestApiServiceBase
    {
        public AzureApiServiceBase(IConfiguration config, ILogger logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient)
            : base(config, logger, tokenAcquisition, httpClient, "https://management.azure.com/.default")
        {

        }        
    }
}
