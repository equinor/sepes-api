using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class ThreadSafeUpdateOperationUtil
    {
        static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public static async Task<List<CloudResourceOperationDto>> CreateDraftRoleUpdateOperationsAsync(Study study,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService)
        {
            try
            {
                await _semaphore.WaitAsync();

                var result = new List<CloudResourceOperationDto>();

                var resourcesToUpdate = CloudResourceUtil.GetSandboxResourceGroupsForStudy(study);
                resourcesToUpdate.AddRange(CloudResourceUtil.GetDatasetResourceGroupsForStudy(study));

                foreach (var currentResource in resourcesToUpdate)
                {
                    var updateOp = await cloudResourceOperationCreateService.CreateUpdateOperationAsync(currentResource.Id, CloudResourceOperationType.ENSURE_ROLES);
                    result.Add(updateOp);
                }

                return result;
            }            
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
