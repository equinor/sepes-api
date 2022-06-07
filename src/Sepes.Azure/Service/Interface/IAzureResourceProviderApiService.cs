using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureResourceProviderApiService
    {
        Task<IList<string>> ListSupportedLocations(string @namespace, string resourceTypeName, CancellationToken cancellationToken = default);
    }
}
