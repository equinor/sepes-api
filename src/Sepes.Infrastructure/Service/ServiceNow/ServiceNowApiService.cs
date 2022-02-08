using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.ServiceNow;
using Sepes.Common.Service;
using Sepes.Common.Service.Interface;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.ServiceNow
{
    public class ServiceNowApiService : RestApiServiceBase, IServiceNowApiService
    {
        public ServiceNowApiService(IConfiguration config, ILogger<ServiceNowApiService> logger, IRequestAuthenticatorWithTokenAquistionService requestAuthenticatorService, HttpClient httpClient)
           : base(config, logger, requestAuthenticatorService, httpClient, ConfigUtil.GetConfigValueAndThrowIfEmpty(config, ConfigConstants.SERVICE_NOW_API_SCOPE), ApiTokenType.App)
        {

        }

        public async Task<ServiceNowResponse> CreateEnquiry(ServiceNowEnquiryCreateDto enquiry, CancellationToken cancellationToken = default)
        {
            var serviceNowApiUrl = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.SERVICE_NOW_API_URL);
            var serviceNowSubscriptionKey = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.SERVICE_NOW_APIM_SUBSCRIPTION);
            var callerId = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.SERVICE_NOW_CALLER_ID);
            var cmdbCi = ConfigUtil.GetConfigValueAndThrowIfEmpty(_config, ConfigConstants.SERVICE_NOW_CMDB_CI);

            var requestBody = new ServiceNowRequest(callerId, enquiry.Category, cmdbCi, enquiry.ShortDescription);
            var httpContent = new StringContent(JsonSerializerUtil.Serialize(requestBody), Encoding.UTF8, "application/json");

            var additionalHeaders = new Dictionary<string, string> { { "Ocp-Apim-Subscription-Key", serviceNowSubscriptionKey } };

            var apiResponse = await PerformRequest<ServiceNowResponse>(serviceNowApiUrl, HttpMethod.Post, httpContent, true, additionalHeaders, cancellationToken);

            return apiResponse;
        }
    }
}
