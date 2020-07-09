using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureVNetService
    {
        Task<INetwork> Create(Region region, string resourceGroupName, string studyName, string sandboxName);

        //Task<string> Status(string resourceGroup, string vNetName);

        Task Delete(string resourceGroup, string vNetName);
    }
}