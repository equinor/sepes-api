using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Dataset : UpdateableBaseModel
    {
        [MaxLength(64)]
        [Required(AllowEmptyStrings =false)]
        public string Name { get; set; }

        [MaxLength(64)]
        [Required(AllowEmptyStrings =false)]
        public string StorageAccountName { get; set; }

        [MaxLength(64)]
        [Required(AllowEmptyStrings =false)]
        public string Location { get; set; }

        [MaxLength(32)]
        [Required(AllowEmptyStrings =false)]
        public string Classification { get; set; }

        public int LRAId { get; set; }
        public int DataId { get; set; }
        public string SourceSystem { get; set; }
        public string BADataOwner { get; set; }
        public string Asset { get; set; }
        public string CountryOfOrigin { get; set; }
        public string AreaL1 { get; set; }
        public string AreaL2 { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }

        // Attributes used for linking.
        // ------------------------------
        public ICollection<StudyDataset> StudyDatasets { get; set; }

        public ICollection<SandboxDataset> SandboxDatasets { get; set; }

        //StudyID is only populated if dataset is StudySpecific.
        //This is accounted for in API calls.
        public int? StudyId { get; set; }
    }
}
