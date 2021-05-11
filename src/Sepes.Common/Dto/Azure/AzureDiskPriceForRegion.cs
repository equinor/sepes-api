using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class AzureDiskPriceForRegion
    {
        public Dictionary<string, DiskType> Types { get; set; } = new Dictionary<string, DiskType>();
    }
    public class DiskType
    {
        public int size { get; set; }
        public double price;
    }
}
