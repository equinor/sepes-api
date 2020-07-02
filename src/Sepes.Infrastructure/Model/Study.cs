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

        public string LogoUrl { get; set; }

        public virtual ICollection<StudyDataset> StudyDatasets { get; set; }

        public virtual ICollection<StudyParticipant> StudyParticipants { get; set; }

        public virtual ICollection<Sandbox> Sandboxes { get; set; }

        public virtual ICollection<AzureResource> AzureResources { get; set; }
    }




}
