using Sepes.Infrastructure.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Dataset : UpdateableBaseModel, ISupportSoftDelete
    {
        [MaxLength(64)]
        [Required(AllowEmptyStrings =false)]
        public string Name { get; set; }

        [MaxLength(64)]        
        public string StorageAccountName { get; set; }

        [MaxLength(256)]
        public string StorageAccountId { get; set; }

        [MaxLength(64)]
        [Required(AllowEmptyStrings =false)]
        public string Location { get; set; }

        [MaxLength(32)]
        [Required(AllowEmptyStrings =false)]
        public string Classification { get; set; }

        public int LRAId { get; set; }
        public int DataId { get; set; }
        [MaxLength(256)]
        public string SourceSystem { get; set; }
        [MaxLength(256)]
        public string BADataOwner { get; set; }
        [MaxLength(256)]
        public string Asset { get; set; }
        [MaxLength(256)]
        public string CountryOfOrigin { get; set; }
        [MaxLength(256)]
        public string AreaL1 { get; set; }
        [MaxLength(256)]
        public string AreaL2 { get; set; }
        [MaxLength(256)]
        public string Tags { get; set; }
        [MaxLength(1024)]
        public string Description { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(64)]
        public string DeletedBy { get; set; }

        // Attributes used for linking.
        // ------------------------------
        public ICollection<StudyDataset> StudyDatasets { get; set; }
        public ICollection<DatasetFirewallRule> FirewallRules { get; set; }
        public ICollection<SandboxDataset> SandboxDatasets { get; set; }

        //StudyID is only populated if dataset is StudySpecific.
        //This is accounted for in API calls.
        public int? StudyId { get; set; }      

        public List<CloudResource> Resources { get; set; }
    }   
}
