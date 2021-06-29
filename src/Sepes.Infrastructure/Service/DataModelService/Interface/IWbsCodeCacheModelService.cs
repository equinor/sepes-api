using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IWbsCodeCacheModelService
    {
        Task Add(string wbsCode, bool valid);
             
        Task<WbsCodeCache> Get(string wbsCode);

        Task Clean();
    }
}
