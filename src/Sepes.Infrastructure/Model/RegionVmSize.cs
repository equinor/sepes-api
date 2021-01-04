namespace Sepes.Infrastructure.Model
{
    public class RegionVmSize
    {
        public string RegionKey { get; set; }

        public string VmSizeKey { get; set; }

        public Region Region { get; set; }

        public VmSize VmSize { get; set; }
        public double Price { get; set; }
    }
}
