using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceOperationReadService : CloudResourceOperationServiceBase, ICloudResourceOperationReadService
    {    
      

        public CloudResourceOperationReadService(SepesDbContext db, IMapper mapper)
            : base(db, mapper)
        {
          
        }

        public async Task<CloudResourceOperationDto> GetByIdAsync(int id)
        {
            return await GetOperationDtoInternal(id);
        }      

        public async Task<bool> OperationIsFinishedAndSucceededAsync(int operationId)
        {
            var itemFromDb = await GetResourceOperationOrThrowAsync(operationId);

            return itemFromDb.Status == CloudResourceOperationState.DONE_SUCCESSFUL;
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
