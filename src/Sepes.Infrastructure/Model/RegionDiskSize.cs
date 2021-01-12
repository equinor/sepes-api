﻿namespace Sepes.Infrastructure.Model
{
    public class RegionDiskSize
    {
        public string RegionKey { get; set; }

        public string VmDiskKey { get; set; }

        public double Price { get; set; }

        public Region Region { get; set; }

        public DiskSize DiskSize { get; set; }
    
    }
}