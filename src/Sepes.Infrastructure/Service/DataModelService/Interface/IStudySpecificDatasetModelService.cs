using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudySpecificDatasetModelService
    { 
        Task<Dataset> GetByIdAsync(int datasetId, UserOperation userOperation);

        Task<Dataset> GetForResourceAndFirewall(int datasetId, UserOperation userOperation);

        Task<Dataset> GetByIdWithoutPermissionCheckAsync(int datasetId);
        Task<bool> IsStudySpecific(int datasetId);
        Task<Dataset> GetByIdWithResourceAndFirewallWithoutPermissionCheckAsync(int datasetId);
    }
}
