using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureBastionService
    {
        //string CreateName(string studyName, string sandboxName);
        Task<string> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string nsgName);
     
        //Task Delete(string resourceGroupName, string securityGroupName);
    }
}