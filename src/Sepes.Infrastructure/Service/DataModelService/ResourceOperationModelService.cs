using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
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

        public ResourceOperationModelService(IConfiguration configuration, SepesDbContext db, ILogger<ResourceOperationModelService> logger, IUserService userService)
            : base(configuration, db, logger, userService)
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
                await CheckAccesAndThrowIfMissing(cloudResourceOperation.Resource.Study, operation);
            }
            else if (cloudResourceOperation.Resource.SandboxId.HasValue)
            {
                await CheckAccesAndThrowIfMissing(cloudResourceOperation.Resource.Sandbox.Study, operation);
            }           
        }
    }
}
