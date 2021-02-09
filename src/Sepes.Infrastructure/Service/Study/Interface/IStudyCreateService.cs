using Sepes.Infrastructure.Dto.Study;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyCreateService
    { 
        Task<StudyDetailsDto> CreateAsync(StudyCreateDto newStudy, CancellationToken cancellation = default);   
    }
}
