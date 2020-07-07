using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IParticipantService
    {       
        Task<IEnumerable<ParticipantListItemDto>> GetLookupAsync();
        Task<ParticipantDto> GetByIdAsync(int id);

        // -----------Applies to study-------------
        Task<StudyDto> AddParticipantToStudyAsync(int studyId, int participantId, string role);
        Task<StudyDto> RemoveParticipantFromStudyAsync(int studyId, int participantId);
    }
}
