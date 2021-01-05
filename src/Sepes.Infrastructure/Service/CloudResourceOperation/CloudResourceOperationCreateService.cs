using AutoMapper;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceOperationCreateService : CloudResourceOperationServiceBase, ICloudResourceOperationCreateService
    {  
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;

        public CloudResourceOperationCreateService(SepesDbContext db, IMapper mapper, IUserService userService, IRequestIdService requestIdService)
            : base(db, mapper)
        {
           
            _userService = userService;
            _requestIdService = requestIdService;
        }

        public async Task<CloudResourceOperationDto> AddAsync(int sandboxResourceId, CloudResourceOperationDto operationDto)
        {
            var sandboxResourceFromDb = await GetResourceOrThrowAsync(sandboxResourceId);
            var newOperation = _mapper.Map<CloudResourceOperation>(operationDto);

            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetOperationDtoInternal(newOperation.Id);
        }

        public async Task<CloudResourceOperationDto> CreateUpdateOperationAsync(int sandboxResourceId, int dependsOn = 0, string batchId = null)
        {
            var sandboxResourceFromDb = await GetResourceOrThrowAsync(sandboxResourceId);

            var currentUser = await _userService.GetCurrentUserAsync();

            if (dependsOn == 0)
            {
                var previousOperation = sandboxResourceFromDb.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

                if (previousOperation != null)
                {
                    if (previousOperation.OperationType == CloudResourceOperationType.DELETE)
                    {
                        throw new Exception($"Error when adding operation to resource {sandboxResourceId}, resource allready marked for deletion");
                    }
                    else if (previousOperation.OperationType == CloudResourceOperationType.UPDATE)
                    {
                        //If previous operation is waiting for some other op, it's best this also gets in line
                        if (previousOperation.DependsOnOperationId.HasValue)
                        {
                            dependsOn = previousOperation.Id;
                        }
                    }
                    else
                    {
                        dependsOn = previousOperation.Id;
                    }
                }
            }

            var newOperation = new CloudResourceOperation()
            {
                Description = AzureResourceUtil.CreateDescriptionForResourceOperation(sandboxResourceFromDb.ResourceType, CloudResourceOperationType.UPDATE, sandboxResourceId),
                BatchId = batchId,
                OperationType = CloudResourceOperationType.UPDATE,
                CreatedBy = currentUser.UserName,
                CreatedBySessionId = _requestIdService.GetRequestId(),
                DependsOnOperationId = dependsOn != 0 ? dependsOn : default(int?),
                MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
            };


            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetOperationDtoInternal(newOperation.Id);
        }

        public async Task<CloudResourceOperation> CreateDeleteOperationAsync(int sandboxResourceId, string description, string batchId = null)
        {
            var user = await _userService.GetCurrentUserAsync();

            var resourceFromDb = await GetResourceOrThrowAsync(sandboxResourceId);

            var deleteOperation = new CloudResourceOperation()
            {
                CreatedBy = user.UserName,  
                BatchId = batchId,
                CloudResourceId = sandboxResourceId,
                OperationType = CloudResourceOperationType.DELETE,
                CreatedBySessionId = _requestIdService.GetRequestId(),
                MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT,
                Description = description
            };

            resourceFromDb.Operations.Add(deleteOperation);

            await _db.SaveChangesAsync();

            return deleteOperation;
        } 
    }
}
