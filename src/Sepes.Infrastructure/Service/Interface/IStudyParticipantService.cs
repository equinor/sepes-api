using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantService
    {       
        Task<IEnumerable<ParticipantListItemDto>> GetLookupAsync();
        Task<ParticipantDto> GetByIdAsync(int id);
       
    }
}
