using Sepes.Infrastructure.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Sandbox : UpdateableBaseModel, ISupportSoftDelete
    { 
        [Required(AllowEmptyStrings =false)]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings =false)]
        [MaxLength(64)]
        public string TechnicalContactName { get; set; }

        [Required(AllowEmptyStrings =false)]
        [MaxLength(128)]
        public string TechnicalContactEmail { get; set; }

        [Required(AllowEmptyStrings =false)]
        [MaxLength(64)]
        public string Region { get; set; }

        public Study Study { get; set; }

        public int StudyId { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(64)]
        public string DeletedBy { get; set; }

        public List<CloudResource> Resources{ get; set; }

        public List<SandboxDataset> SandboxDatasets { get; set; }

        public List<SandboxPhaseHistory> PhaseHistory { get; set; }
    }   
}
