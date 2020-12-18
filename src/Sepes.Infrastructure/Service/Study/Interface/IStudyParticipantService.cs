using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantService
    {       
        Task<IEnumerable<ParticipantLookupDto>> GetLookupAsync(string searchText, int limit = 30);     
        Task<StudyParticipantDto> AddAsync(int studyId, ParticipantLookupDto user, string role);
        Task<StudyParticipantDto> RemoveAsync(int studyId, int userId, string roleName);
    }
}
