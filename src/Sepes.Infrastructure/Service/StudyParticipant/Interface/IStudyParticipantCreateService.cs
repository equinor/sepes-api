using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantCreateService
    { 
        Task<StudyParticipantDto> AddAsync(int studyId, ParticipantLookupDto user, string role);      
    }
}
