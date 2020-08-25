using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IHasTags
    {
        Task<IEnumerable<KeyValuePair<string, string>>> GetTags(string resourceGroupName, string resourceName);
        Task UpdateTag(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag);
    }
}
