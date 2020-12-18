using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync(bool? excludeHidden = null);
        Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation);

        Task<StudyDetailsDto> GetStudyDetailsDtoByIdAsync(int studyId, UserOperation userOperation);        

        Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId);   

   
    }
}
