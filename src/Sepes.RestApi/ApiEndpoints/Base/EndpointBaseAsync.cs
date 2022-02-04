using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Base
{
    public static class EndpointBaseAsync
    {
        public static class WithRequest<TRequest>
        {
            public abstract class WithResult<TResponse> : EndpointBase
            {
                public abstract Task<TResponse> HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithoutResult : EndpointBase
            {
                public abstract Task HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult<TResponse> : EndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult : EndpointBase
            {
                public abstract Task<ActionResult> HandleAsync(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }
        }

        public static class WithoutRequest
        {
            public abstract class WithResult<TResponse> : EndpointBase
            {
                public abstract Task<TResponse> HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithoutResult : EndpointBase
            {
                public abstract Task HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult<TResponse> : EndpointBase
            {
                public abstract Task<ActionResult<TResponse>> HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult : EndpointBase
            {
                public abstract Task<ActionResult> HandleAsync(
                    CancellationToken cancellationToken = default
                );
            }
        }
    }
}
