using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceDeleteService
    {           
        Task HandleSandboxDeleteAsync(int sandboxId);     
    }   
}
