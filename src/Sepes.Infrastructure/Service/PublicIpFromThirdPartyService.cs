using Sepes.Common.Util;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class PublicIpFromThirdPartyService : IPublicIpFromThirdPartyService
    {

        public PublicIpFromThirdPartyService()
        {

        }

        public async Task<string> GetIp(string curIpUrl, CancellationToken cancellation = default)
        {
            using (var client = new HttpClient())
            {

                HttpResponseMessage responseMessage = null;

                try
                {
                    responseMessage = await client.GetAsync(curIpUrl, cancellation);

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var deserializedResponse = JsonSerializerUtil.Deserialize<IpAddressResponse>(await responseMessage.Content.ReadAsStringAsync());

                        return deserializedResponse.ip;
                    }
                    else
                    {
                        throw new Exception(await CreateErrorMessage(curIpUrl, responseMessage: responseMessage));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(await CreateErrorMessage(curIpUrl, responseMessage: responseMessage, exception: ex), ex);

                }

            }

            throw new Exception(await CreateErrorMessage(curIpUrl));
        }

        async Task<string> CreateErrorMessage(string url, HttpResponseMessage responseMessage = null, Exception exception = null)
        {
            var resultBuilder = new StringBuilder($"Request to {url} failed.");

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

    class IpAddressResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string ip { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
