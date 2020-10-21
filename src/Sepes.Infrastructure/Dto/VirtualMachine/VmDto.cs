using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class VmDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Region { get; set; }

        public string LastKnownProvisioningState { get; set; }

        //Todo:
        //Add OS, Distro
        //Status
        //Disks
        //Created, updated
        //Nic
        
    }
}
