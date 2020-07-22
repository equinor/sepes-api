using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto
{
    public class SandboxResourceOperationDto : UpdateableBaseDto
    {
        public int SandboxResourceId { get; set; }

        public string Status { get; set; }

        public int TryCount { get; set; }

        public int DependsOn { get; set; }

        public string SessionId { get; set; }
    }
}
