using Sepes.Common.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantSearchService
    {       
        Task<IEnumerable<ParticipantLookupDto>> SearchAsync(string searchText, int limit = 30, CancellationToken cancellationToken = default);          
    }
}
