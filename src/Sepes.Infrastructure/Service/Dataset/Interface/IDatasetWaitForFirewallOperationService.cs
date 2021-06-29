using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetWaitForFirewallOperationService
    {
        Task WaitForOperationToCompleteAsync(int operationId);
    }
}
