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

        public Study Study { get; set; }

        public int StudyId { get; set; }

        public List<SandboxResource> Resources{ get; set; }
    }
}
