using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IWbsCodeCacheModelService
    {
        Task Add(string wbsCode);
        Task Clean();
        Task<bool> Exists(string wbsCode, CancellationToken cancellation = default);
    }
}
