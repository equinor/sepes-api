using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Constants;
using Sepes.Common.Service;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : RestApiServiceBase, IWbsValidationService
    {
        readonly string _subscriptionId;        

        public WbsValidationService(IConfiguration config, ILogger<WbsValidationService> logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient)
          : base(config, logger, tokenAcquisition, httpClient)
        {
            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];
        } 
        
        public async Task<bool> Exists(string wbsCode, CancellationToken cancellation)
        {
            return await PerformRequestAsync(wbsCode, cancellation);
        }

        async Task<bool> PerformRequestAsync(string wbsCode, CancellationToken cancellation)
        {
            var wbsApiUrl = $"";
            var additionalHeaders = new Dictionary<string, string> { { "Ocp-Apim-Subscription-Key", "" } };
            var result = await PerformRequest<WbsApiResponse>(wbsApiUrl, HttpMethod.Get, needsAuth: true, additionalHeaders: additionalHeaders, cancellationToken: cancellation);
            return true;
        }
    }  

    class WbsApiResponse
    {
        public string code { get; set; }
    }
}