using Sepes.RestApi.Model;
using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        Task<string> CreateResourceGroup(PodInput pod);
        Task TerminateResourceGroup(string resourceGroupName);
        Task<string> CreateNetwork(string name, string address);
    }
}
