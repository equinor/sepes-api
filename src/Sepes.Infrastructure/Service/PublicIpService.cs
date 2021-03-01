using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class PublicIpService : IPublicIpService
    {
        readonly IConfiguration _configuration;
        readonly ILogger _logger;

        string _serverPublicIp;

        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public PublicIpService(IConfiguration configuration, ILogger<PublicIpService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GetServerPublicIp()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(_serverPublicIp))
                {
                    await _semaphore.WaitAsync();
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
            var tryCount = 1;

            try
            {
                while (tryCount < 3)
                {
                    using (var client = new HttpClient())
                    {
                        var ipUrls = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.SERVER_PUBLIC_IP_URLS);

                        var errorMessageSb = new StringBuilder();

                        foreach (var curIpUrl in ipUrls)
                        {
                            HttpResponseMessage responseMessage = null;

                            try
                            {
                                responseMessage = await client.GetAsync(curIpUrl);

                                if (responseMessage.IsSuccessStatusCode)
                                {
                                    var responseString = await responseMessage.Content.ReadAsStringAsync();

                                    var deserializedResponse = JsonConvert.DeserializeObject<IpAddressResponse>(await responseMessage.Content.ReadAsStringAsync());

                                    return deserializedResponse.ip;
                                }
                                else
                                {
                                    errorMessageSb.AppendLine(await GetPublicIpErrorMessage(curIpUrl, responseMessage: responseMessage));
                                }
                            }
                            catch (Exception ex)
                            {
                                errorMessageSb.AppendLine(await GetPublicIpErrorMessage(curIpUrl, responseMessage: responseMessage, exception: ex));
                            }
                        }                      
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed getting server public IP. Attempt {tryCount}");
                tryCount++;               
            }                  

            throw new Exception($"Failed to get server public IP after {tryCount} tries.");
        }

        async Task<string> GetPublicIpErrorMessage(string url, HttpResponseMessage responseMessage = null, Exception exception = null)
        {
            var resultBuilder = new StringBuilder($"Failed to get server public ip from {url}.");

            if (responseMessage != null)
            {
                resultBuilder.AppendLine($" Status code: {responseMessage.StatusCode}, reason: {responseMessage.ReasonPhrase}");

                var responseString = await responseMessage.Content.ReadAsStringAsync();

                if (!String.IsNullOrWhiteSpace(responseString))
                {
                    resultBuilder.AppendLine($" Response from server: {responseString}");
                }
            }

            if (exception != null)
            {
                resultBuilder.AppendLine($" Exception from server: {exception}");
            }

            return resultBuilder.ToString();
        }
    }

    public class IpAddressResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string ip { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
