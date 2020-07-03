using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureVNetService
    {
        Task<INetwork> Create(Region region, string resourceGroupName, string studyName, string sandboxName);
        string CreateVNetName(string studyName, string sandboxName);
        Task Delete(string resourceGroup, string vNetName);
    }
}