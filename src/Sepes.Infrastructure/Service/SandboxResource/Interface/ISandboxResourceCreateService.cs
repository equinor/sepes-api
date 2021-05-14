using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceCreateService
    {
        Task CreateBasicSandboxResourcesAsync(Sandbox sandbox);  
    }   
}
