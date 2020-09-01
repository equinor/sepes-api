using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Sandbox : UpdateableBaseModel
    { 
        [Required]
        public string Name { get; set; }

        [Required]
        public string TechnicalContactName { get; set; }

        [Required]
        public string TechnicalContactEmail { get; set; }

        [Required]
        public string Region { get; set; }

        public Study Study { get; set; }

        public int StudyId { get; set; }

        public bool? Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string DeletedBy { get; set; }

        public List<SandboxResource> Resources{ get; set; }
    }
}
