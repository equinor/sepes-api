using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Region : StringKeyBaseModel
    {
        [MaxLength(32)]
        public string Name { get; set; }

        public bool Disabled { get; set; }

        public ICollection<RegionVmSize> VmSizeAssociations { get; set; }

        public ICollection<RegionDiskSize> DiskSizeAssociations { get; set; }
    }
}
