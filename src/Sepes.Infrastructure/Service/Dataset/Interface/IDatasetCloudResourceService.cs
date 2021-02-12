using Sepes.Infrastructure.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetCloudResourceService
    {
        Task CreateResourceGroupForStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);

        Task CreateResourcesForStudySpecificDatasetAsync(Dataset dataset, string clientIp, CancellationToken cancellationToken = default);

        Task EnsureFirewallExistsAsync(Study study, Dataset dataset, string clientIp, CancellationToken cancellationToken = default);      

        Task DeleteAllStudyRelatedResourcesAsync(Study study, CancellationToken cancellationToken = default);

        Task DeleteResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, CancellationToken cancellationToken = default);
    }
}
