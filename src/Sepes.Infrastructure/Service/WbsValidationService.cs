using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Common.Constants;
using Sepes.Common.Service;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WbsValidationService : RestApiServiceBase, IWbsValidationService
    {
        readonly IUserService _userService;

        public WbsValidationService(IConfiguration config, ILogger<WbsValidationService> logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient, IUserService userService)
          : base(config, logger, tokenAcquisition, httpClient, ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.WBS_SEARCH_API_SCOPE), ApiTokenType.User)
        {
            _userService = userService;
        }

        public async Task<bool> Exists(string wbsCode, CancellationToken cancellation = default)
        {
            StudyAccessUtil.HasAccessToOperationOrThrow(await _userService.GetCurrentUserAsync(), UserOperation.Study_Create);

            return await PerformRequestAsync(wbsCode, cancellation);
        }

        async Task<bool> PerformRequestAsync(string wbsCode, CancellationToken cancellation)
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
        public string code { get; set; }
    }
}