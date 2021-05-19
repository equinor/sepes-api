using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceOperationReadService : CloudResourceOperationServiceBase, ICloudResourceOperationReadService
    {      

        public CloudResourceOperationReadService(SepesDbContext db, IMapper mapper, IUserService userService)
            : base(db, mapper, userService)
        {
          
        }

        public async Task<CloudResourceOperationDto> GetByIdAsync(int id)
        {
            return await GetOperationDtoInternal(id);
        }      

        public async Task<bool> OperationIsFinishedAndSucceededAsync(int operationId)
        {
            var itemFromDb = await GetResourceOperationOrThrowAsync(operationId, true);

            return itemFromDb.Status == CloudResourceOperationState.DONE_SUCCESSFUL;
        }

        public async Task<bool> OperationFailedOrAbortedAsync(int operationId)
        {
            var itemFromDb = await GetResourceOperationOrThrowAsync(operationId, true);
            return (itemFromDb.Status == CloudResourceOperationState.FAILED && itemFromDb.TryCount >= itemFromDb.MaxTryCount) || itemFromDb.Status == CloudResourceOperationState.ABORTED || itemFromDb.Status == CloudResourceOperationState.ABANDONED;
        }

        public async Task<CloudResourceOperation> GetUnfinishedDeleteOperation(int resourceId)
        {
            return await _db.CloudResourceOperations
               .Where(o => o.CloudResourceId == resourceId
               && (String.IsNullOrWhiteSpace(o.Status) || o.Status == CloudResourceOperationState.NEW || o.Status == CloudResourceOperationState.IN_PROGRESS)
               ).FirstOrDefaultAsync();
        }      
    }
}
