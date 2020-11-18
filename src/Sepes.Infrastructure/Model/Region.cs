using System.Collections.Generic;

namespace Sepes.Infrastructure.Model
{
    public class Region : StringKeyBaseModel
    {
        public string Name { get; set; }

        public bool Disabled { get; set; }

        public ICollection<RegionVmSize> VmSizeAssociations { get; set; }
    }
}
