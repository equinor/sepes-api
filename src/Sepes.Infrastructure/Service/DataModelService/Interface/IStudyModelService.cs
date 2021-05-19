using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyModelService
    {
        Task<Study> AddAsync(Study study);

        Task<IEnumerable<StudyListItemDto>> GetListAsync();

        Task<Study> GetByIdAsync(int studyId, UserOperation userOperation);        

        Task<Study> GetForStudyDetailsAsync(int studyId);     

        Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId);
        Task<Study> GetForDatasetsAsync(int studyId, UserOperation operation = UserOperation.Study_Read);

        Task<Study> GetForDatasetCreationAsync(int studyId, UserOperation operation);

        Task<Study> GetForDatasetCreationNoAccessCheckAsync(int studyId);
        Task<Study> GetForParticpantOperationsAsync(int studyId, UserOperation operation, string newRole = null);
        Task<Study> GetForSandboxCreateAndDeleteAsync(int studyId, UserOperation operation);
        Task<Study> GetWitParticipantsNoAccessCheck(int studyId);
        Task<Study> GetForDeleteAsync(int studyId, UserOperation operation);
        Task<Study> GetForCloseAsync(int studyId, UserOperation operation);
    }
}
