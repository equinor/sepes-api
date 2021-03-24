using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class VirtualMachineDiskSizeService : ModelServiceBase, IVirtualMachineDiskSizeService
    { 
        readonly ISandboxModelService _sandboxModelService;

        public VirtualMachineDiskSizeService(IConfiguration configuration, SepesDbContext db, ILogger<VirtualMachineDiskSizeService> logger, IUserService userService, ISandboxModelService sandboxModelService)
            :base(configuration, db, logger, userService)
        {  
            _sandboxModelService = sandboxModelService;
        }

        public async Task<IEnumerable<VmDiskLookupDto>> AvailableDisks(int sandboxId, CancellationToken cancellationToken = default)
        {
            var sandboxRegion = await _sandboxModelService.GetRegionByIdAsync(sandboxId, UserOperation.Study_Crud_Sandbox);

            var disksQuery = "SELECT DISTINCT [Id] as [StudyId], [Name], [Description], [Vendor], [Restricted], [LogoUrl] FROM [dbo].[Studies] s";
            disksQuery += " INNER JOIN [dbo].[StudyParticipants] sp on s.Id = sp.StudyId";
            disksQuery += " WHERE s.Closed = 0";     


            var disks = await RunDapperQueryMultiple<VmDiskLookupDto>(disksQuery);
            return disks;

            //return await _db.Regions.Include(r=> r.DiskSizeAssociations).Where(r=> r.Key == sandboxRegion).Select(r=> r.VmSizeAssociations).Select(r => new VmDiskLookupDto() { Key = r..Key, DisplayValue = r.DisplayText }).AsNoTracking().ToListAsync();
        }       
    }
}
