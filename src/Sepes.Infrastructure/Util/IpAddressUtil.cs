using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class IpAddressUtil
    {
        public static async Task<string> GetServerPublicIp()
        {
            using (var client = new HttpClient())
            {
                var responseMessage = await client.GetAsync("https://api.ipify.org?format=json");

                var responseString = await responseMessage.Content.ReadAsStringAsync();

                var deserializedResponse = JsonConvert.DeserializeObject<IpAddressResponse>(await responseMessage.Content.ReadAsStringAsync());

                return deserializedResponse.ip;
            }
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
