using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureUserService
    {
        Task<List<Microsoft.Graph.User>> SearchUsersAsync(string search, int limit, CancellationToken cancellationToken = default);

        Task<AzureUserDto> GetUserAsync(string id);
    }
}
