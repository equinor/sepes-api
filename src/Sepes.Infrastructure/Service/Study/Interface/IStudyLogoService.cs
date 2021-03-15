using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyLogoService
    {
        Task DecorateLogoUrlsWithSAS(IEnumerable<StudyListItemDto> studyDtos);

        Task DecorateLogoUrlWithSAS(IHasLogoUrl hasLogo);

        Task DeleteAsync(Study study);

        //Task<StudyDetailsDto> AddLogoAsync(int id, IFormFile studyLogo);

        Task<string> AddLogoAsync(int id, IFormFile studyLogo);

        //Task<LogoResponseDto> GetLogoAsync(int id);
    }
}