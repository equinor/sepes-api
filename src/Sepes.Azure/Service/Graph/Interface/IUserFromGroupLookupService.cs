using Sepes.Azure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IUserFromGroupLookupService
    {
        Task<Dictionary<string, AzureUserDto>> SearchInGroupAsync(string groupId, string search, int limit, CancellationToken cancellationToken = default);
    }
}
