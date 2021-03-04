using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Study;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyCreateService
    { 
        Task<StudyDetailsDto> CreateAsync(StudyCreateDto newStudy, [FromForm(Name = "image")] IFormFile logo = null, CancellationToken cancellation = default);   
    }
}
