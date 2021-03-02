using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class PublicIpWithCacheAndRetryService : IPublicIpWithCacheAndRetryService
    {
        readonly IConfiguration _configuration;
        readonly ILogger _logger;
        readonly IPublicIpService _publicIpService;

        string _serverPublicIp;

        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public PublicIpWithCacheAndRetryService(IConfiguration configuration, ILogger<PublicIpWithCacheAndRetryService> logger, IPublicIpService publicIpService)
        {
            _configuration = configuration;
            _logger = logger;
            _publicIpService = publicIpService;
        }

        public async Task<string> GetServerPublicIp()
        {
            try
            {
                await _semaphore.WaitAsync();

                if (String.IsNullOrWhiteSpace(_serverPublicIp))
                {                   
                    _serverPublicIp = await GetServerPublicIpFromExternalService();
                }

                return _serverPublicIp;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        async Task<string> GetServerPublicIpFromExternalService()
        {
            var tryCount = 0;

            try
            {
                do
                {
                    tryCount++;

                    var ipUrls = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.SERVER_PUBLIC_IP_URLS);

                    var errorMessageSb = new StringBuilder();

                    foreach (var curIpUrl in ipUrls)
                    {
                        try
                        {
                            var publicIpFromExternalService = await _publicIpService.GetServerPublicIp(curIpUrl);
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
                _logger.LogError(ex, $"Failed to get public ip. Attempt {tryCount}");
                tryCount++;
            }

            throw new Exception($"Failed to get public ip after {tryCount} tries.");
        }     
    }
}
