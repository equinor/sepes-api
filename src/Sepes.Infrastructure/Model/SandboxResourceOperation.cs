﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class SandboxResourceOperation : UpdateableBaseModel
    {        
        public int SandboxResourceId { get; set; }
     
        public string OperationType { get; set; }

        public string Status { get; set; }

        public int TryCount { get; set; }

        public string CreatedBySessionId { get; set; }

        public string CarriedOutBySessionId { get; set; }

        public string Description { get; set; }

        public SandboxResource Resource { get; set; }
    }
}
