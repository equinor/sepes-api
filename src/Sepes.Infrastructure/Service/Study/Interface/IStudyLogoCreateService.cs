using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyLogoCreateService
    { 
        Task<string> CreateAsync(int id, IFormFile studyLogo);    
    }
}