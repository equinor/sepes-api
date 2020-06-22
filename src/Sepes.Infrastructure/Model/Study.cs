using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Study : UpdateableBaseModel
    {
        [MaxLength(128)]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [MaxLength(64)]
        public string WbsCode { get; set; }

        [MaxLength(128)]
        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public ICollection<StudyDataset> StudyDatasets { get; set; }

        public ICollection<Sandbox> SandBoxes { get; set; }

        public int? LogoId { get; set; }

        public StudyLogo Logo { get; set; }

    }

    public class StudyDataset
    {
        public int StudyId { get; set; }

        public Study Study { get; set; }

        public int DatasetId { get; set; }

        public Dataset Dataset { get; set; }
    }
}
