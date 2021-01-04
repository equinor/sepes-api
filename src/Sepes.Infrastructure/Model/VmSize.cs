using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class VmSize : StringKeyBaseModel
    {
        [MaxLength(32)]
        public string Category { get; set; }

        [MaxLength(128)]
        public string DisplayText { get; set; }

        public int NumberOfCores { get; set; }

        public int OsDiskSizeInMB { get; set; }

        public int ResourceDiskSizeInMB { get; set; }

        public int MemoryGB { get; set; }

        public int MaxDataDiskCount { get; set; }

        public int MaxNetworkInterfaces { get; set; }

        public double Price { get; set; }

        public ICollection<RegionVmSize> RegionAssociations { get; set; }
    }
}
