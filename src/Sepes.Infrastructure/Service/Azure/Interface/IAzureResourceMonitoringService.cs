using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureResourceMonitoringService
    {        
        Task CheckForOrphanResources();
        Task StartMonitoringSession();

        Task<string> GetProvisioningState(SandboxResourceDto resource);
    }
}