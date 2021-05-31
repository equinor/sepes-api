using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Constants;
using Sepes.Common.Service;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsApiService : RestApiServiceBase, IWbsApiService
    {

        public WbsApiService(IConfiguration config, ILogger<WbsApiService> logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient)
          : base(config, logger, tokenAcquisition, httpClient, ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.WBS_SEARCH_API_SCOPE), ApiTokenType.User)
        {
            
        }

        public async Task<bool> Exists(string wbsCode, CancellationToken cancellation = default)
        {
            try
            {
                var wbsApiUrlFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.WBS_SEARCH_API_URL);
                var wbsApiUrl = $"{wbsApiUrlFromConfig}?code={wbsCode}&skip=0&top=100&api-version=1.0";

                var wbsApimSubscriptionKey = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.WBS_SEARCH_APIM_SUBSCRIPTION);

                var additionalHeaders = new Dictionary<string, string> { { "Ocp-Apim-Subscription-Key", wbsApimSubscriptionKey } };

                var apiResponse = await PerformRequest<List<WbsApiResponse>>(wbsApiUrl, HttpMethod.Get, needsAuth: true, additionalHeaders: additionalHeaders, cancellationToken: cancellation);

                return ApiResponseContainsWbsCode(apiResponse, wbsCode);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Wbs validation failed for code {wbsCode}");
            }

            return false;
        }      

        bool ApiResponseContainsWbsCode(List<WbsApiResponse> response, string wbsCode)
        {
            if (response != null)
            {
                if (response.Count > 1)
                {
                    return false;
                }

                var wbsFromResponse = response.SingleOrDefault();

                if (wbsFromResponse != null)
                {
                    if (wbsFromResponse.code.ToLowerInvariant() == wbsCode.ToLowerInvariant())
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    class WbsApiResponse
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        public string code { get; set; }
    }
}