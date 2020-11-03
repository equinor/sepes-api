using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantService
    {       
        Task<IEnumerable<ParticipantLookupDto>> GetLookupAsync(string searchText, int limit = 30);

        Task<StudyParticipantDto> RemoveParticipantFromStudyAsync(int studyId, int userId, string roleName);
        Task<StudyParticipantDto> HandleAddParticipantAsync(int studyId, ParticipantLookupDto user, string role);
    }
}
