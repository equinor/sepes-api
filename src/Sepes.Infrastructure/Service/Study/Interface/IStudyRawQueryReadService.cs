using Sepes.Common.Dto.Study;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyRawQueryReadService
    {        
        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync();          
    }
}
