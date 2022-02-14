using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Base
{
    public static class EndpointBase
    {
        public static class WithRequest<TRequest>
        {
            public abstract class WithResult<TResponse> : ControllerBase
            {
                public abstract Task<TResponse> Handle(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithoutResult : ControllerBase
            {
                public abstract Task Handle(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult<TResponse> : ControllerBase
            {
                public abstract Task<ActionResult<TResponse>> Handle(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult : ControllerBase
            {
                public abstract Task<ActionResult> Handle(
                    TRequest request,
                    CancellationToken cancellationToken = default
                );
            }
        }

        public static class WithoutRequest
        {
            public abstract class WithResult<TResponse> : ControllerBase
            {
                public abstract Task<TResponse> Handle(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithoutResult : ControllerBase
            {
                public abstract Task Handle(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult<TResponse> : ControllerBase
            {
                public abstract Task<ActionResult<TResponse>> Handle(
                    CancellationToken cancellationToken = default
                );
            }

            public abstract class WithActionResult : ControllerBase
            {
                public abstract Task<ActionResult> Handle(
                    CancellationToken cancellationToken = default
                );
            }
        }
    }
}
