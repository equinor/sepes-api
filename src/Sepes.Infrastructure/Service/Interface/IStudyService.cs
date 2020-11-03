using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudiesAsync(bool? excludeHidden = null);
        Task<StudyDto> GetStudyByIdAsync(int studyId);

        Task<StudyDto> CreateStudyAsync(StudyCreateDto newStudy);

        Task<StudyDto> UpdateStudyDetailsAsync(int studyId, StudyDto newStudy);

        Task<StudyDto> DeleteStudyAsync(int studyId);

        Task<StudyDto> AddLogoAsync(int id, IFormFile studyLogo);

        Task<byte[]> GetLogoAsync(int id);   
    }
}
