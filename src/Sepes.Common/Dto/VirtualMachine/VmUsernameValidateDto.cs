using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Dto.VirtualMachine
{
    public class VmUsernameValidateDto
    {
        public Boolean isValid { get; set; }
        public string errorMessage { get; set; }
    }
}
