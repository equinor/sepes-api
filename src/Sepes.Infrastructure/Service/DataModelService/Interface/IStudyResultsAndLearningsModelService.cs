using Sepes.Common.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyResultsAndLearningsModelService
    { 
        Task<StudyResultsAndLearningsDto> GetAsync(int studyId);
    }
}
