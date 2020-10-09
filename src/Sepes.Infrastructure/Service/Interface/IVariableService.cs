using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVariableService
    {
        Task<Variable> GetByNameAsync(string name);
    }
}
