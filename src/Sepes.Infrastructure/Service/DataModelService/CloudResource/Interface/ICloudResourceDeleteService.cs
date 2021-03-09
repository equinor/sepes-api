using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ICloudResourceDeleteService
    {         
        Task<CloudResourceDto> MarkAsDeletedAsync(int resourceId);

        Task HardDeletedAsync(int resourceId);

        Task<CloudResourceOperationDto> MarkAsDeletedWithDeleteOperationAsync(int resourceId, UserOperation operation);
    }
}
