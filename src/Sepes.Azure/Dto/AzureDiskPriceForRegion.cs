using System.Collections.Generic;

namespace Sepes.Azure.Dto
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
