using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmSizeDto
    {
        public string Name { get; set; }

        public int NumberOfCores { get; set; }

        public int OsDiskSizeInMB { get; set; }

        public int ResourceDiskSizeInMB { get; set; }

        public int MemoryInMB { get; set; }

        public int MaxDataDiskCount { get; set; }

        public Region Region { get; set; }
    }
}
