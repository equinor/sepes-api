using Microsoft.AspNetCore.Http;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyLogoService
    {
        Task DecorateLogoUrlsWithSAS(IEnumerable<IHasLogoUrl> studyDtos);

        Task DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo);

        Task DeleteAsync(Study study);      

        Task<string> AddLogoAsync(int id, IFormFile studyLogo);    
    }
}