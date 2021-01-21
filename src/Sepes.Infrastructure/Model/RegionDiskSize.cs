using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sepes.Infrastructure.Model
{
    public class RegionDiskSize
    {
        [MaxLength(64)]
        public string RegionKey { get; set; }

        [MaxLength(64)]
        public string VmDiskKey { get; set; }

        public double Price { get; set; }

        public Region Region { get; set; }

        public DiskSize DiskSize { get; set; }
    
    }
}
