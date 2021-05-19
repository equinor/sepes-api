using Sepes.Common.Dto;
using Sepes.Common.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantCreateService
    { 
        Task<StudyParticipantDto> AddAsync(int studyId, ParticipantLookupDto user, string role);      
    }
}
