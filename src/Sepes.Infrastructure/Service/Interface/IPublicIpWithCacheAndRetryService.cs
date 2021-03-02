using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IPublicIpWithCacheAndRetryService
    {
        Task<string> GetServerPublicIp();
    }
}
