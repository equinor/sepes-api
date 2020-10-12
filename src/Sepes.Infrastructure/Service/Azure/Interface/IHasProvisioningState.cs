using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IHasProvisioningState
    {
        Task<string> GetProvisioningState(string resourceGroupName, string resourceName);
    }
}
