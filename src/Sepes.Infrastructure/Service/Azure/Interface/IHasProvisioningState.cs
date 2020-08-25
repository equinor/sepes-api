using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IHasProvisioningState
    {
        Task<string> GetProvisioningState(string resourceGroupName, string resourceName);
    }
}
