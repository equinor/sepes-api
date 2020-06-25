using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Study : UpdateableBaseModel
    {
        [MaxLength(128)]
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [MaxLength(64)]
        public string WbsCode { get; set; }

        [MaxLength(128)]
        [Required]
        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public virtual ICollection<StudyDataset> StudyDatasets { get; set; }

        public virtual ICollection<Sandbox> Sandboxes { get; set; }

        public int? LogoId { get; set; }

        public StudyLogo Logo { get; set; }

    }

    public class StudyDataset
    {
        public int StudyId { get; set; }
        public virtual Study Study { get; set; }

        public int DatasetId { get; set; }
        public virtual Dataset Dataset { get; set; }
    }
}
