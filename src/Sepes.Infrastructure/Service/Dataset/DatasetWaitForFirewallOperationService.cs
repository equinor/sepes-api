using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetWaitForFirewallOperationService : WaitForOperationService, IDatasetWaitForFirewallOperationService
    {
        public DatasetWaitForFirewallOperationService(ICloudResourceOperationReadService cloudResourceOperationReadService)
            : base(cloudResourceOperationReadService)
        {
        
        }


        public async Task WaitForOperationToCompleteAsync(int operationId)
        {
            await base.WaitForOperationToCompleteAsync(operationId);
        }
    }
}

