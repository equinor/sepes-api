﻿using Microsoft.EntityFrameworkCore;
using Sepes.Common.Dto;
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

        public RegionService(SepesDbContext db)
        {
            _db = db;      
        }

        public async Task<IEnumerable<LookupDto>> GetLookup(CancellationToken cancellationToken = default)
        {
           return await _db.Regions.AsNoTracking().Where(r => !r.Disabled).OrderBy(r => r.Order).Select(r=> new LookupDto() { Key = r.Key, DisplayValue = r.Name }).ToListAsync(cancellationToken);          
        }
    }
}
