using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudySpecificDatasetService
    {         
        Task<StudyDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputBaseDto newDataset, CancellationToken cancellationToken = default);

        Task<StudyDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputBaseDto newDataset);

        Task SoftDeleteStudySpecificDatasetAsync(int datasetId, CancellationToken cancellationToken = default);
        Task SoftDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default);
        Task HardDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default);

        Task HardDeleteStudySpecificDatasetAsync(int datasetId, CancellationToken cancellationToken = default);
        Task SoftDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);
        Task HardDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);
    }
}
