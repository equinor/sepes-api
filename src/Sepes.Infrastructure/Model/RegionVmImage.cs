using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class RegionVmImage
    {
        [MaxLength(64)]
        public string RegionKey { get; set; }
        
        public int VmImageId { get; set; }

        public Region Region { get; set; }

        public VmImage VmImage { get; set; }     
    }
}
