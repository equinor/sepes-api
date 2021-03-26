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
        public VirtualMachineDiskSizeService(IConfiguration configuration, SepesDbContext db, ILogger<VirtualMachineDiskSizeService> logger, IUserService userService)
            :base(configuration, db, logger, userService)
        {  
          
        }

        public async Task<IEnumerable<VmDiskLookupDto>> AvailableDisks(int sandboxId, CancellationToken cancellationToken = default)
        {
            var disksQuery = "WITH sandboxRegionCte AS (SELECT [Region] from dbo.[Sandboxes] where Id = @SandboxId)";
            disksQuery += " ,diskSizesCte AS (SELECT [Key], [DisplayText] as [DisplayValue], [Size] FROM [dbo].[RegionDiskSize] rd";
            disksQuery += " LEFT JOIN [dbo].[DiskSizes] d on rd.[VmDiskKey] = d.[Key]";
            disksQuery += " WHERE rd.[RegionKey] = (SELECT [Region] from sandboxRegionCte)";
            disksQuery += " ) SELECT [Key], [DisplayValue] FROM diskSizesCte ORDER BY [Size]";

            var disks = await RunDapperQueryMultiple<VmDiskLookupDto>(disksQuery, new { SandboxId = sandboxId } );
            return disks;            
        }       
    }
}
