using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IHasTags
    {
        Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName);
        Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag);
    }
}
