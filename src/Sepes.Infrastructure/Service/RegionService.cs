using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class RegionService : IRegionService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public RegionService(SepesDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LookupDto>> GetLookup(CancellationToken cancellationToken = default)
        {
            var regionsFromDb = await _db.Regions.Where(r => !r.Disabled).ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<LookupDto>> (regionsFromDb);
        }
    }
}
