using Sepes.Common.Constants;
using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ICloudResourceDeleteService
    {         
        Task<CloudResourceDto> MarkAsDeletedAsync(int resourceId);

        Task HardDeleteAsync(int resourceId);

        Task<CloudResourceOperationDto> MarkAsDeletedWithDeleteOperationAsync(int resourceId, UserOperation operation);
    }
}
