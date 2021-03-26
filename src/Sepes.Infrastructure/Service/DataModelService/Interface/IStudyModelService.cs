using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyModelService
    {
        Task<Study> AddAsync(Study study);

        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync();

        Task<Study> GetByIdAsync(int studyId, UserOperation userOperation);

        Task<Study> GetStudyForStudyDetailsAsync(int studyId);
     

        Task<StudyResultsAndLearningsDto> GetStudyResultsAndLearningsAsync(int studyId);
        Task<Study> GetStudyForDatasetsAsync(int studyId, UserOperation operation = UserOperation.Study_Read);

        Task<Study> GetStudyForDatasetCreationAsync(int studyId, UserOperation operation);

        Task<Study> GetStudyForDatasetCreationNoAccessCheckAsync(int studyId);
        Task<Study> GetStudyForParticpantOperationsAsync(int studyId, UserOperation operation, string newRole = null);
        Task<Study> GetStudyForSandboxCreationAsync(int studyId, UserOperation operation);
    }
}
