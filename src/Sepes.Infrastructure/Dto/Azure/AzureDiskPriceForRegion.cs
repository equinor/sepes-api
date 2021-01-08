using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class AzureDiskPriceForRegion
    {
        public Dictionary<string, DiskType> Types { get; set; } = new Dictionary<string, DiskType>();
    }

    public class DiskType
    {
        public double price;
    }
}
