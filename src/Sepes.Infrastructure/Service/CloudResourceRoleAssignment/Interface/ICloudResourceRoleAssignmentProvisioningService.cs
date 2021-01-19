using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceRoleAssignmentProvisioningService
    {
        Task<CloudResourceOperationDto> DefineAndOrderForSandbox(int sandboxId);

        //Task CreateForSandbox(int sandboxId);




    }
}