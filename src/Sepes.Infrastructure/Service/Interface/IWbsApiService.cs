using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IWbsApiService
    {
        Task<bool> Exists(string wbsCode, CancellationToken cancellation = default);
    }
}
