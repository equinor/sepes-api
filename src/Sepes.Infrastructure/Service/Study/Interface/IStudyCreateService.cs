using Microsoft.AspNetCore.Http;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyCreateService
    { 
        Task<Study> CreateAsync(StudyCreateDto newStudy, IFormFile logo = null, CancellationToken cancellation = default);   
    }
}
