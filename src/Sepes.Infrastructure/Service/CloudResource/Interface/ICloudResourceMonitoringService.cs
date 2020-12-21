using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceMonitoringService
    {        
        Task CheckForOrphanResources();
        Task StartMonitoringSession();

        Task<string> GetProvisioningState(SandboxResourceDto resource);
    }
}