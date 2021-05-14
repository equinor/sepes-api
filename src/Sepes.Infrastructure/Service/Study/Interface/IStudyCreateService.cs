using Microsoft.AspNetCore.Http;
using Sepes.Common.Dto.Study;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyCreateService
    { 
        Task<StudyDetailsDto> CreateAsync(StudyCreateDto newStudy, IFormFile logo = null, CancellationToken cancellation = default);   
    }
}
