using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceOperationCreateService
    {
        Task<CloudResourceOperationDto> AddAsync(int sandboxResourceId, CloudResourceOperationDto operationDto);

        Task<CloudResourceOperationDto> CreateUpdateOperationAsync(int sandboxResourceId, int dependsOn = 0, string batchId = null);
     
      
        Task<CloudResourceOperation> CreateDeleteOperationAsync(int sandboxResourceId, string description, string batchId = null);
    }
}