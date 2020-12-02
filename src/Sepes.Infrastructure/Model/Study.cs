using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Study : UpdateableBaseModel
    {
        [MaxLength(128)]
        [Required(AllowEmptyStrings =false)]
        public string Name { get; set; }
        
        public string Description { get; set; }

        public string ResultsAndLearnings { get; set; }

        [MaxLength(64)]
        public string WbsCode { get; set; }

        [MaxLength(128)]
        [Required(AllowEmptyStrings =false)]
        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public string LogoUrl { get; set; }

        public bool? Closed { get; set; }

        public DateTime? ClosedAt{ get; set; }

        [MaxLength(64)]
        public string ClosedBy { get; set; }

        public virtual ICollection<StudyDataset> StudyDatasets { get; set; }

        public virtual ICollection<StudyParticipant> StudyParticipants { get; set; }

        public virtual ICollection<Sandbox> Sandboxes { get; set; }

        public virtual ICollection<SandboxResource> CloudResources { get; set; }
    }
}
