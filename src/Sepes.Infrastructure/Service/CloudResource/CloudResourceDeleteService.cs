using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceDeleteService : CloudResourceServiceBase, ICloudResourceDeleteService
    {
        readonly IRequestIdService _requestIdService;
        readonly ICloudResourceOperationService _sandboxResourceOperationService;

        public CloudResourceDeleteService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceDeleteService> logger, IUserService userService, IRequestIdService requestIdService, ICloudResourceOperationService sandboxResourceOperationService)
         : base(db, config, mapper, logger, userService)
        {           
            _requestIdService = requestIdService;
            _sandboxResourceOperationService = sandboxResourceOperationService;

        }

        public async Task<SandboxResourceOperationDto> MarkAsDeletedAsync(int id)
        {
            var user = await _userService.GetCurrentUserAsync();

            var resourceFromDb = await GetOrThrowInternalAsync(id);

            var deleteOperationDescription = $"Delete resource {id} ({resourceFromDb.ResourceType})";

            _logger.LogInformation($"{deleteOperationDescription}: Abort all other operations for Resource");

            await _sandboxResourceOperationService.AbortAllUnfinishedCreateOrUpdateOperations(id);

            var deleteOperation = await _sandboxResourceOperationService.GetUnfinishedDeleteOperation(id);

            if (deleteOperation == null)
            {
                _logger.LogInformation($"{deleteOperationDescription}: Creating delete operation");

                deleteOperation = new CloudResourceOperation()
                {
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(resourceFromDb.ResourceType, CloudResourceOperationType.DELETE, resourceFromDb.SandboxId, id),
                    CreatedBy = user.UserName,
                    BatchId = Guid.NewGuid().ToString(),
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    OperationType = CloudResourceOperationType.DELETE,
                    CloudResourceId = resourceFromDb.Id,
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                };

                resourceFromDb.Operations.Add(deleteOperation);
            }
            else
            {
                _logger.LogInformation($"{deleteOperationDescription}: Existing delete operation found, re-queueing that");
                deleteOperation.TryCount = 0;
            }

            MarkAsDeletedInternal(resourceFromDb, user.UserName);

            await _db.SaveChangesAsync();         

            _logger.LogInformation($"{deleteOperationDescription}: Done!");

            return _mapper.Map<SandboxResourceOperationDto>(deleteOperation);
        }

        async Task<CloudResource> MarkAsDeletedByIdInternalAsync(int id)
        {
            var resourceEntity = await _db.SandboxResources.FirstOrDefaultAsync(s => s.Id == id);

            if (resourceEntity == null)
            {
                throw NotFoundException.CreateForEntity("SandboxResource", id);
            }

            var user = await _userService.GetCurrentUserAsync();

            MarkAsDeletedInternal(resourceEntity, user.UserName);

            await _db.SaveChangesAsync();

            return resourceEntity;
        }

        CloudResource MarkAsDeletedInternal(CloudResource resource, string deletedBy)
        {
            resource.DeletedBy = deletedBy;
            resource.Deleted = DateTime.UtcNow;

            return resource;
        }
    }
}
