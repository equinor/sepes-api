using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class HealthService : IHealthService
    {       
        readonly SepesDbContext _db;
        readonly ILogger _logger;
        readonly IPublicIpService _publicIpService;

        public HealthService(ILogger<HealthService> logger, SepesDbContext db, IPublicIpService publicIpService)
        {           
            _db = db;
            _logger = logger;
            _publicIpService = publicIpService;
        }

        public async Task<HealthSummaryResponse> GetHealthSummaryAsync(HttpContext context, CancellationToken cancellation = default)
        {
            var response = new HealthSummaryResponse()
            {
                DatabaseConnectionOk = await DatabaseOkayAsync(cancellation),
                IpAddresses = await GetIPsAsync(context, cancellation),
                Headers = CleanHeaders(context)
            };

            return response;
        }

        Dictionary<string, string> CleanHeaders(HttpContext context)
        {
            var result = new Dictionary<string, string>();

            foreach(var curHeader in context.Request.Headers)
            {
                if(curHeader.Key == "Authorization")
                {
                    result.Add(curHeader.Key, curHeader.Value.ToString().Substring(0, 10));
                }
                else
                {
                    result.Add(curHeader.Key, curHeader.Value);
                }          
            }
            return result;
        }

        async Task<bool> DatabaseOkayAsync(CancellationToken cancellation = default)
        {
            try
            {
                _ = await _db.Variables.FirstOrDefaultAsync(cancellation);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Health check for database failed");
            }

            return false;
        }

        async Task<Dictionary<string, string>> GetIPsAsync(HttpContext context, CancellationToken cancellation = default)
        {
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;
                var localIpAddress = context.Connection.LocalIpAddress;

                var response = new Dictionary<string, string>()
                     {
                    { "Server public ip from 3rd party", await _publicIpService.GetIp() },
                    { "context.Connection.RemoteIpAddress", remoteIpAddress.ToString() },
                    { "context.Connection.RemoteIpAddress MapToIPv4", remoteIpAddress.MapToIPv4().ToString() },
                    { "context.Connection.RemoteIpAddress MapToIPv6", remoteIpAddress.MapToIPv6().ToString() },
                    { "context.Connection.RemoteIpAddress AddressFamily", remoteIpAddress.AddressFamily.ToString() },
                    { "context.Connection.LocalIpAddress", localIpAddress.ToString() },
                    { "context.Connection.LocalIpAddress MapToIPv4", localIpAddress.MapToIPv4().ToString() },
                    { "context.Connection.LocalIpAddress MapToIPv6", localIpAddress.MapToIPv6().ToString() },
                    { "context.Connection.LocalIpAddress AddressFamily", localIpAddress.AddressFamily.ToString() },
                };

                var counter = 0;

                foreach (var curDns in Dns.GetHostEntry(remoteIpAddress).AddressList)
                {
                    try
                    {
                        var description = $"{curDns} | family: {curDns.AddressFamily}";

                        if (curDns.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                        {
                            description += $" | v4: {curDns.MapToIPv4()}";
                        }

                        response.Add($"fromDns[{counter}]", description);
                        counter++;
                    }
                    catch (Exception)
                    {

                     
                    }
                 
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Health check for Client IPs failed");
            }

            return null;
        }      
    }
}
