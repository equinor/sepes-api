using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmExtendedDto
    {
        public string PowerState { get; set; }

        public string OsType { get; set; }

        public string SizeName { get; set; }

        public string PublicIp { get; set; }

        public string PrivateIp { get; set; } 

        public VmSizeDto Size { get; set; }

        public List<VmDiskDto> Disks { get; set; } = new List<VmDiskDto>();

        public List<VmNicDto> NICs { get; set; } = new List<VmNicDto>();
    }
}
