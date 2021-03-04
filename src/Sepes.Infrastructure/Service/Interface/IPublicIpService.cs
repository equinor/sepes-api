using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IPublicIpService
    {
        Task<string> GetIp(CancellationToken cancellation = default);      
    }
}
