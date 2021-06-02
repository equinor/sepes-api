using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyEfReadService
    { 
        Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation);

        Task<StudyDetailsDto> GetStudyDetailsAsync(int studyId);            
    }
}
