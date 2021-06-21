using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ICloudResourceUpdateService
    {
        //GENERAL METHODS        

        Task<CloudResource> Update(int resourceId, CloudResource updated);   


        //MORE SPECIFIC RESOURCE OPERATIONS     

        Task<CloudResourceDto> UpdateResourceGroup(int resourceId, CloudResourceDto updated);


        Task<CloudResourceDto> UpdateResourceIdAndName(int resourceId, string azureId, string azureName);

        Task UpdateProvisioningState(int resourceId, string newProvisioningState);

        Task UpdateTagsAsync(int resourceId, Dictionary<string, string> tags);
    }
}
