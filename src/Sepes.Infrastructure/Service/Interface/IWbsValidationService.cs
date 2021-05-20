using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IWbsValidationService
    {
        Task<bool> IsValid(string wbsCode, CancellationToken cancellation = default);
    }
}
