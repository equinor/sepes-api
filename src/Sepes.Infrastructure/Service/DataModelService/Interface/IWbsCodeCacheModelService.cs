using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IWbsCodeCacheModelService
    {
        Task Add(string wbsCode, bool valid);
        Task Clean();
        Task<bool> ExistsAndValid(string wbsCode, CancellationToken cancellation = default);
    }
}
