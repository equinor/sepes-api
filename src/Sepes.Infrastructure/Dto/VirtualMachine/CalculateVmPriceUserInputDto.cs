﻿using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.VirtualMachine
{
    public class CalculateVmPriceUserInputDto
    { 
        public string Size { get; set; }

        public List<string> DataDisks { get; set; }

        public string OperatingSystem { get; set; }      
    }
}
