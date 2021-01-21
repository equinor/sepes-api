using System;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class SandboxDataset
    {             
        public int SandboxId { get; set; }
        public virtual Sandbox Sandbox { get; set; }

        public int DatasetId { get; set; }
        public virtual Dataset Dataset { get; set; }
        [MaxLength(64)]
        public string AddedBy { get; set; }

        public DateTime Added { get; set; }
    }
}
