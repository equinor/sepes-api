using Sepes.Azure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Graph
{
    public interface IUserFromGroupRequest
    {
        public string SearchName { get; }

        public string SearchText { get; }

        Task<Dictionary<string, AzureUserDto>> StartRequest(CancellationToken cancellationToken = default);
    }
}
