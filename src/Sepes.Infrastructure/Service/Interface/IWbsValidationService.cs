using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IWbsValidationService
    {
        Task<bool> Exists(string wbsCode, CancellationToken cancellation);
    }
}
