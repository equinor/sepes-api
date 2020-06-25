using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Participant : UpdateableBaseModel
    { 
        [Required]
        public string Name { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public ICollection<StudyParticipant> StudyParticipants { get; set; }
    }
}
