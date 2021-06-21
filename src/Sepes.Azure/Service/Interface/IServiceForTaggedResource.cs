using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IServiceForTaggedResource
    {
        Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName);
        Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag, CancellationToken cancellationToken = default);

        Task SetTagsAsync(string resourceGroupName, string resourceName, Dictionary<string, string> tags, CancellationToken cancellationToken = default);
    }
}
