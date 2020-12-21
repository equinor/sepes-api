using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceUpdateService
    {
        //GENERAL METHODS        

        Task<SandboxResourceDto> Update(int resourceId, SandboxResourceDto updated);   


        //MORE SPECIFIC RESOURCE OPERATIONS     

        Task<SandboxResourceDto> UpdateResourceGroup(int resourceId, SandboxResourceDto updated);


        Task<SandboxResourceDto> UpdateResourceIdAndName(int resourceId, string azureId, string azureName);

        Task UpdateProvisioningState(int resourceId, string newProvisioningState);
    }
}
