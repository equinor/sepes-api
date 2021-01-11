using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Model
{
    public class RegionDiskSize
    {
        public string RegionKey { get; set; }

        public string VmDiskKey { get; set; }

        public Region Region { get; set; }

        public DiskSize DiskSize { get; set; }
        public double Price { get; set; }
    }
}
