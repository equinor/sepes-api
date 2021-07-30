using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Common.Service.Interface
{
    public interface IRestApiTokenAquisitionService
    {
        Task<string> GetAccessTokenForAppAsync(string scope);
    
        Task<string> GetAccessTokenForUserAsync(IEnumerable<string> scopes);
    }
}
