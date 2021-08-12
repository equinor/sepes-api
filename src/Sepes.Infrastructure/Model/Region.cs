using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Region : StringKeyBaseModel
    {
        [MaxLength(32)]
        public string Name { get; set; }

        [MaxLength(32)]
        public string KeyInPriceApi { get; set; }

        public bool Disabled { get; set; }

        public ICollection<RegionVmSize> VmSizeAssociations { get; set; }

        public ICollection<RegionDiskSize> DiskSizeAssociations { get; set; }

        public ICollection<RegionVmImage> VmImageAssociations { get; set; }
        
        public int Order { get; set; }
    }
}
