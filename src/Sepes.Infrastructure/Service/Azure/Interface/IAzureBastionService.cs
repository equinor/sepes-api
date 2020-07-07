using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureBastionService
    {
      
        Task<BastionHostInner> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string nsgName);
     
        //Task Delete(string resourceGroupName, string securityGroupName);
    }
}