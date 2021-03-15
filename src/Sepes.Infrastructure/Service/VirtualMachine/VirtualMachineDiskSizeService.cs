using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineDiskSizeService : IVirtualMachineDiskSizeService
    {       
        readonly SepesDbContext _db;      

        public VirtualMachineDiskSizeService(SepesDbContext db)
        {           
            _db = db;
        }

        public async Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default)
        {
            return await _db.DiskSizes.OrderBy(x => x.Size).Select(ds => new VmDiskLookupDto() { Key = ds.Key, DisplayValue = ds.DisplayText }).AsNoTracking().ToListAsync();
        }       
    }
}
