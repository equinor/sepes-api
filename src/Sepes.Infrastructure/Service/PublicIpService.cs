using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class PublicIpService : IPublicIpService
    {
        readonly ILogger _logger;
        readonly IConfiguration _configuration;        
        readonly IPublicIpFromThirdPartyService _publicIpFromThirdPartyService;

        string _cachedValue;

        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public PublicIpService(IConfiguration configuration, ILogger<PublicIpService> logger, IPublicIpFromThirdPartyService publicIpFromThirdPartyService)
        {
            _logger = logger;
            _configuration = configuration;       
            _publicIpFromThirdPartyService = publicIpFromThirdPartyService;
        }

        public async Task<string> GetIp(CancellationToken cancellation = default)
        {
            try
            {
                await _semaphore.WaitAsync();

                if (String.IsNullOrWhiteSpace(_cachedValue))
                {
                    _cachedValue = await GetFromThirdParty(cancellation);
                }

                return _cachedValue;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        async Task<string> GetFromThirdParty(CancellationToken cancellation = default)
        {
            var tryCount = 0;

            try
            {
                do
                {
                    tryCount++;

                    var thirdPartyUrls = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.SERVER_PUBLIC_IP_URLS);                  

                    foreach (var curUrl in thirdPartyUrls)
                    {
                        try
                        {
                            var publicIpFromExternalService = await _publicIpFromThirdPartyService.GetIp(curUrl, cancellation);                         
                            return publicIpFromExternalService;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Failed to get public ip. Attempt {tryCount}");                         
                        }
                    }

                } while (tryCount < 3);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get public ip. Gave up after {tryCount} attempts");               
            }

            throw new Exception($"Failed to get IP");
        }       
    }
}
