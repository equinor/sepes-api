using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyReadService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync();

        Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation);

        Task<StudyDetailsDto> GetStudyDetailsAsync(int studyId);        

        Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId);    
    }
}
