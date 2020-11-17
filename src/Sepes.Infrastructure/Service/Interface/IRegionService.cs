using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IRegionService
    {
        public Task<IEnumerable<LookupDto>> GetLookup(CancellationToken cancellationToken = default);
    }
}
