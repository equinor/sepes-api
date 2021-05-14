using Sepes.Common.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Dto.VirtualMachine
{
    public class VmExternalLink :IHasLinkToExtSystem
    {
        public int Id { get; set; }
        public string LinkToExternalSystem { get; set; }
    }
}
