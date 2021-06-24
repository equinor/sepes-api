using Microsoft.AspNetCore.Http;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyUpdateService
    { 
        Task<Study> UpdateMetadataAsync(int studyId, StudyUpdateDto newStudy, IFormFile logo = null, CancellationToken cancellationToken = default);
        Task<StudyResultsAndLearningsDto> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings);
    }
}
