using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Sepes.RestApi.Util
{
    public static class IpAddressUtil
    {      

        public static string GetClientIp(HttpContext context)
        {
            if (GetForwardedForHeader(context, out string forwarderFor))
            {
                return forwarderFor;
            }

            if (GetRealIpHeader(context, out string realIp))
            {
                return realIp;
            }

            var remoteIpAddress = context.Connection.RemoteIpAddress;

            if(remoteIpAddress == null)
            {
                return null;
            }

            return remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ?
                remoteIpAddress.MapToIPv4().ToString() :
                remoteIpAddress.ToString();

        }      

        static bool GetForwardedForHeader(HttpContext context, out string headerValue)
        {
            if (GetRequestHeaderValue(context, "X-Forwarded-For", out headerValue))
            {
                return true;
            }

            return false;
        }

        static bool GetRealIpHeader(HttpContext context, out string headerValue)
        {
            if (GetRequestHeaderValue(context, "X-Real-IP", out headerValue))
            {
                return true;
            }

            return false;
        }

        static bool GetRequestHeaderValue(HttpContext context, string headerName, out string headerValue)
        {
            StringValues headerValueTmp;

            if (context.Request.Headers.TryGetValue(headerName, out headerValueTmp))
            {
                headerValue = headerValueTmp.ToString();
                return true;
            }

            headerValue = null;
            return false;
        }
    }

  
}
