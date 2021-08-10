using Sepes.Common.Dto;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudySpecificDatasetService
    {         
        Task<StudySpecificDatasetDto> CreateStudySpecificDatasetAsync(int studyId, DatasetCreateUpdateInputBaseDto newDataset, string clientIp, CancellationToken cancellationToken = default);

        Task<StudySpecificDatasetDto> UpdateStudySpecificDatasetAsync(int studyId, int datasetId, DatasetCreateUpdateInputBaseDto newDataset);

        Task DeleteAllStudyRelatedResourcesAsync(Study study, CancellationToken cancellationToken = default);

        Task SoftDeleteStudySpecificDatasetAsync(int datasetId, CancellationToken cancellationToken = default);
        Task SoftDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default);
        Task HardDeleteStudySpecificDatasetAsync(Study study, int datasetId, CancellationToken cancellationToken = default);

        Task HardDeleteStudySpecificDatasetAsync(int datasetId, CancellationToken cancellationToken = default);
        Task SoftDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);
        Task HardDeleteAllStudySpecificDatasetsAsync(Study study, CancellationToken cancellationToken = default);
        Task<List<DatasetResourceLightDto>> GetDatasetResourcesAsync(int studyId, int datasetId, CancellationToken cancellation);
        Task<StudySpecificDatasetDto> GetDatasetAsync(int studyId, int datasetId);
    }
}
