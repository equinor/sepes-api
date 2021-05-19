using Sepes.Common.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantRemoveService
    {   
        Task<StudyParticipantDto> RemoveAsync(int studyId, int userId, string roleName);
    }
}
