using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sepes.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class IpAddressUtil
    {
        public static async Task<string> GetServerPublicIp(IConfiguration config)
        {
            using (var client = new HttpClient())
            {
                var ipUrls = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(config, ConfigConstants.SERVER_PUBLIC_IP_URLS);

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

                throw new Exception("Could not get Server Public ip. " + errorMessageSb.ToString());
            }
        }

        static async Task<string> GetPublicIpErrorMessage(string url, HttpResponseMessage responseMessage = null, Exception exception = null)
        {
            var resultBuilder = new StringBuilder($"Failed to get server public ip from {url}.");

            if(responseMessage != null)
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

        public static string GetClientIp(HttpContext context)
        {
            var clientIp = context.Connection.RemoteIpAddress.ToString();
            return clientIp;
        }

        public static void EnsureListContainsIpAddress(List<string> list, string ipAddress)
        {
            if (!list.Contains(ipAddress))
            {
                list.Add(ipAddress);
            }
        }

    }

    public class IpAddressResponse
    {
        public string ip { get; set; }
    }
}
