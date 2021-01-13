using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class DiskSize: StringKeyBaseModel
    {
        public int Size { get; set; }

        [MaxLength(128)]
        public string DisplayText { get; set; }

        public ICollection<RegionDiskSize> RegionAssociations { get; set; }
    }
}
