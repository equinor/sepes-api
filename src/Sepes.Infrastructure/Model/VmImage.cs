using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class VmImage : UpdateableBaseModel
    {
        [MaxLength(1024)]
        public string ForeignSystemId { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string DisplayValue { get; set; }

        [MaxLength(128)]
        public string DisplayValueExtended { get; set; }

        [MaxLength(64)]
        public string Category { get; set; }

        public bool Recommended { get; set; }

        public ICollection<RegionVmImage> RegionAssociations { get; set; }
    }
}
