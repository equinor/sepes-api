using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Constants;
using Sepes.Common.Service;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : RestApiServiceBase, IWbsValidationService
    {             

        public WbsValidationService(IConfiguration config, ILogger<WbsValidationService> logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient)
          : base(config, logger, tokenAcquisition, httpClient, ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.WBS_SEARCH_API_SCOPE))
        {
            
        } 
        
        public async Task<bool> Exists(string wbsCode, CancellationToken cancellation = default)
        {
            return await PerformRequestAsync(wbsCode, cancellation);
        }

        async Task<bool> PerformRequestAsync(string wbsCode, CancellationToken cancellation)
        {
            var wbsApiUrlFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.WBS_SEARCH_API_URL);
            var wbsApiUrl = $"{wbsApiUrlFromConfig}&code={wbsCode}";

            var wbsApimSubscriptionKey = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.WBS_SEARCH_APIM_SUBSCRIPTION);

            var additionalHeaders = new Dictionary<string, string> { { "Ocp-Apim-Subscription-Key", wbsApimSubscriptionKey } };
            var result = await PerformRequest<WbsApiResponse>(wbsApiUrl, HttpMethod.Get, needsAuth: true, additionalHeaders: additionalHeaders, cancellationToken: cancellation);
           
            //TODO: Add real implementation
            return true;
        }
    }  

    class WbsApiResponse
    {
        public string code { get; set; }
    }
}