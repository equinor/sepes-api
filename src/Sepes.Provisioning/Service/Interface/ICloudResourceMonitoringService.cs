using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
    public interface ICloudResourceMonitoringService
    {        
        Task CheckForOrphanResources();
        Task StartMonitoringSession();

        Task<string> GetProvisioningState(CloudResourceDto resource);
    }
}