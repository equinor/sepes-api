using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceUpdateService
    {
        //GENERAL METHODS        

        Task<CloudResourceDto> Update(int resourceId, CloudResourceDto updated);   


        //MORE SPECIFIC RESOURCE OPERATIONS     

        Task<CloudResourceDto> UpdateResourceGroup(int resourceId, CloudResourceDto updated);


        Task<CloudResourceDto> UpdateResourceIdAndName(int resourceId, string azureId, string azureName);

        Task UpdateProvisioningState(int resourceId, string newProvisioningState);
    }
}
