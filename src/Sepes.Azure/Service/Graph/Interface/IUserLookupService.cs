using Sepes.Azure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IUserLookupService
    {
        Task<Dictionary<string, AzureUserDto>> SearchAsync(string search, int limit, CancellationToken cancellationToken = default);      
    }
}
