using Sepes.Infrastructure.Dto.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceDeleteService
    { 
        //DESTRUCTION
        Task<SandboxResourceOperationDto> MarkAsDeletedAsync(int resourceId);
    }
}
