using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class RegionVmSize
    {
        [MaxLength(64)]
        public string RegionKey { get; set; }
        [MaxLength(64)]
        public string VmSizeKey { get; set; }

        public Region Region { get; set; }

        public VmSize VmSize { get; set; }
        public double Price { get; set; }
    }
}
