using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineDeleteService
    { 
        Task DeleteAsync(int id);        
    }
}
