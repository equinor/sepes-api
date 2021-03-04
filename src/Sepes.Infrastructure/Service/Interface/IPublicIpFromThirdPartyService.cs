using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IPublicIpFromThirdPartyService
    {
        Task<string> GetIp(string url, CancellationToken cancellation = default);
    }
}
