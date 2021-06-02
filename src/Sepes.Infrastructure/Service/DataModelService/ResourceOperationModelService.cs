using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class ResourceOperationModelService : DatasetModelServiceBase, IResourceOperationModelService
    {      

        public ResourceOperationModelService(IConfiguration configuration, SepesDbContext db, ILogger<ResourceOperationModelService> logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, db, logger, userService, studyPermissionService)
        {
          
        }

        public async Task<CloudResourceOperation> GetForOperationPromotion(int id)
        {
            var queryable = _db.CloudResourceOperations                
                 .Include(o => o.DependantOnThisOperation)
                 .ThenInclude(o => o.Resource);

            var result = await GetFromQueryableThrowIfNotFound(queryable, id);
            return result;
        }
        
        public async Task<CloudResourceOperation> EnsureReadyForRetry(CloudResourceOperation operationToRetry)
        {
            if (operationToRetry.TryCount >= operationToRetry.MaxTryCount)
            {
                operationToRetry.MaxTryCount += CloudResourceConstants.RESOURCE_MAX_TRY_COUNT; //Increase max try count 
                operationToRetry.Status = CloudResourceOperationState.IN_PROGRESS;
                await _db.SaveChangesAsync();
            }

            return operationToRetry;
        }

        async Task<CloudResourceOperation> GetFromQueryableThrowIfNotFound(IQueryable<CloudResourceOperation> queryable, int id)
        {
            var cloudResourceOperation = await queryable.SingleOrDefaultAsync(s => s.Id == id);

            if (cloudResourceOperation == null)
            {
                throw NotFoundException.CreateForEntity("CloudResourceOperation", id);
            }

            return cloudResourceOperation;
        }

        async Task<CloudResourceOperation> GetFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<CloudResourceOperation> queryable, int id, UserOperation operation)
        {
            var cloudResourceOperation = await GetFromQueryableThrowIfNotFound(queryable, id);

            await CheckAccesAndThrowIfMissing(cloudResourceOperation, operation);

            return cloudResourceOperation;
        }

        async Task CheckAccesAndThrowIfMissing(CloudResourceOperation cloudResourceOperation, UserOperation operation)
        {
            if (cloudResourceOperation.Resource.StudyId.HasValue)
            {
                await CheckAccesAndThrowIfNotAllowed(cloudResourceOperation.Resource.Study, operation);
            }
            else if (cloudResourceOperation.Resource.SandboxId.HasValue)
            {
                await CheckAccesAndThrowIfNotAllowed(cloudResourceOperation.Resource.Sandbox.Study, operation);
            }           
        }
    }
}
