using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyDetailsModelService
    {
        Task<StudyDetailsDapper> GetStudyDetailsAsync(int studyId);

        Task<IEnumerable<SandboxForStudyDetailsDapper>> GetSandboxForStudyDetailsAsync(int studyId);       
        Task<IEnumerable<DatasetForStudyDetailsDapper>> GetDatasetsForStudyDetailsAsync(int studyId);      
        Task<IEnumerable<StudyParticipantForStudyDetailsDapper>> GetParticipantsForStudyDetailsAsync(int studyId);

        Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId);
    }
}
