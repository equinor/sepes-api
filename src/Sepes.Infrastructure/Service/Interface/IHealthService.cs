﻿using Microsoft.AspNetCore.Http;
using Sepes.Common.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IHealthService
    {
        Task<bool> DatabaseOkayAsync(CancellationToken cancellation = default);
        Task<HealthSummaryResponse> GetHealthSummaryAsync(HttpContext context, CancellationToken cancellation = default);
    }
}
