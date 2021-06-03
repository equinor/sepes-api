using Sepes.Common.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyRawQueryModelService
    { 
        Task<IEnumerable<StudyListItemDto>> GetListAsync();         

        Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId);       
    }
}
