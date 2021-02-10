using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class HealthService : IHealthService
    {
        readonly IConfiguration _config;
        readonly SepesDbContext _db;
        readonly ILogger _logger;

        public HealthService(IConfiguration config, ILogger<HealthService> logger, SepesDbContext db)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }

        public async Task<HealthSummaryResponse> GetHealthSummaryAsync(HttpContext context, CancellationToken cancellation = default)
        {
            var response = new HealthSummaryResponse()
            {
                DatabaseConnectionOk = await DatabaseOkayAsync(cancellation),
                IpAddresses = await GetIPsAsync(context, cancellation)
            };

            return response;
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

                return new Dictionary<string, string>()
                {
                    { "Server public ip from 3rd party", await IpAddressUtil.GetServerPublicIp(_config) },
                    { "context.Connection.RemoteIpAddress", remoteIpAddress.ToString() },
                    { "context.Connection.RemoteIpAddress MapToIPv4", remoteIpAddress.MapToIPv4().ToString() },
                    { "context.Connection.RemoteIpAddress MapToIPv6", remoteIpAddress.MapToIPv6().ToString() },
                    { "context.Connection.RemoteIpAddress AddressFamily", remoteIpAddress.AddressFamily.ToString() },

                    { "context.Connection.LocalIpAddress", localIpAddress.ToString() },
                    { "context.Connection.LocalIpAddress MapToIPv4", localIpAddress.MapToIPv4().ToString() },
                    { "context.Connection.LocalIpAddress MapToIPv6", localIpAddress.MapToIPv6().ToString() },
                    { "context.Connection.LocalIpAddress AddressFamily", localIpAddress.AddressFamily.ToString() },

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Health check for Client IPs failed");
            }

            return null;
        }
    }
}
