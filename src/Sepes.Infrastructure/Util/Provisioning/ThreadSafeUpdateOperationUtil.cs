using Sepes.Infrastructure.Constants.CloudResource;
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

        public static async Task<List<int>> CreateDraftRoleUpdateOperationAsync(Study study,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService)
        {
            try
            {
                await _semaphore.WaitAsync();

                var result = new List<int>();

                foreach (var resourceGroup in CloudResourceUtil.GetResourceGroups(study))
                {
                    var updateOp = await cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES);
                    result.Add(updateOp.Id);
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
