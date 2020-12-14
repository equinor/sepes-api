using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IAzureUserService
    {
        Task<List<Microsoft.Graph.User>> SearchUsersAsync(string search, int limit);

        Task<AzureUserDto> GetUserAsync(string id);
    }
}
