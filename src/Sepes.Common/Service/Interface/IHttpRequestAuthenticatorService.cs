using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Common.Service.Interface
{
    public interface IHttpRequestAuthenticatorService
    {
        Task PrepareRequestForAppAsync(HttpRequestMessage httpRequestMessage, string scope, CancellationToken cancellationToken = default);

        Task PrepareRequestForUserAsync(HttpRequestMessage httpRequestMessage, IEnumerable<string> scopes, CancellationToken cancellationToken = default);
    }
}
