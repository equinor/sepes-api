using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IHasProvisioningState
    {
        Task<string> GetProvisioningState(string resourceGroupName, string resourceName);
    }
}
