using Sepes.Common.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyDetailsService
    {        
        Task<StudyDetailsDto> Get(int studyId);          
    }
}
