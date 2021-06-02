using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class VirtualMachineDiskSizeService : DapperModelServiceBase, IVirtualMachineDiskSizeService
    { 
        public VirtualMachineDiskSizeService(IConfiguration configuration, ILogger<VirtualMachineDiskSizeService> logger, IUserService userService)
            :base(configuration, logger, userService)
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
