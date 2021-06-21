using Sepes.Infrastructure.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IWbsCodeCacheModelService
    {
        Task Add(string wbsCode, bool valid);
        Task Clean();
        //Task<bool> ExistsAndValid(string wbsCode, CancellationToken cancellation = default);
        Task<WbsCodeCache> Get(string wbsCode, CancellationToken cancellation = default);
    }
}
