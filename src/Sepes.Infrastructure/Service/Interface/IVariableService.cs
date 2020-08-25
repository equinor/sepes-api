using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IVariableService
    {
        Task<Variable> GetByNameAsync(string name);
    }
}
