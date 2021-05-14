using Microsoft.AspNetCore.Http;
using Sepes.Common.Dto.Study;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyUpdateService
    { 
        Task<StudyDetailsDto> UpdateMetadataAsync(int studyId, StudyUpdateDto newStudy, IFormFile logo = null);
        Task<StudyResultsAndLearningsDto> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings);
    }
}
