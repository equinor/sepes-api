using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync(bool? excludeHidden = null);
        Task<StudyDto> GetStudyDtoByIdAsync(int studyId, UserOperation userOperation);

        Task<StudyDetailsDto> GetStudyDetailsDtoByIdAsync(int studyId, UserOperation userOperation);

        Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudy);

        Task<StudyDto> UpdateStudyMetadataAsync(int studyId, StudyDto newStudy);

        Task CloseStudyAsync(int studyId);
        Task DeleteStudyAsync(int studyId);

        Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo);

        Task<LogoResponseDto> GetLogoAsync(int id);
    }
}
