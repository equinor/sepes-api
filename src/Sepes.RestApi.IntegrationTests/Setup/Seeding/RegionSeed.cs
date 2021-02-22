using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class RegionSeed
    {
        public static async Task Seed()
        {          
                var region = new Infrastructure.Model.Region()
                {
                    Created = DateTime.UtcNow,
                    CreatedBy = "seed",
                    Key = "norwayeast",
                    Name = "Norway East",
                    DiskSizeAssociations = new List<Infrastructure.Model.RegionDiskSize>() {
                    new Infrastructure.Model.RegionDiskSize(){ DiskSize = new Infrastructure.Model.DiskSize() { Key = "standardssd-e1", DisplayText = "4 GB", Size = 4 } },
                    new Infrastructure.Model.RegionDiskSize(){ DiskSize = new Infrastructure.Model.DiskSize() { Key = "standardssd-e2", DisplayText = "8 GB", Size = 8 } }
                },
                    VmSizeAssociations = new List<Infrastructure.Model.RegionVmSize>() {
                    new Infrastructure.Model.RegionVmSize(){ VmSize = new Infrastructure.Model.VmSize() { Key = "Standard_F1" } },
                    new Infrastructure.Model.RegionVmSize(){ VmSize = new Infrastructure.Model.VmSize() { Key = "Standard_F2" } },
                }
                };

                await SliceFixture.InsertAsync(region);         
          
        }
    }
}
