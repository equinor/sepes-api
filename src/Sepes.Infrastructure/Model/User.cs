using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class User : UpdateableBaseModel
    { 
        [Required]
        public string FullName { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string TenantId { get; set; }

        public string ObjectId { get; set; }

        public ICollection<StudyParticipant> StudyParticipants { get; set; }
    }
}
