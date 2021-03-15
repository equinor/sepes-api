using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ICloudResourceReadService
    {
        Task<CloudResource> GetByIdAsync(int id, UserOperation operation);

        Task<CloudResource> GetByIdNoAccessCheckAsync(int id);


        //Task<CloudResourceDto> GetDtoByIdAsync(int id);

        Task<IEnumerable<CloudResource>> GetDeletedResourcesAsync();

        Task<List<CloudResource>> GetAllActiveResources();

        Task<bool> ResourceIsDeleted(int resourceId);    

    }
}
