using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class User : UpdateableBaseModel
    {
        [MaxLength(128)]
        [Required(AllowEmptyStrings =false)]      
        public string FullName { get; set; }

        [MaxLength(128)]
        public string UserName { get; set; }

        [MaxLength(256)]
        public string EmailAddress { get; set; }

        [MaxLength(64)]
        public string ObjectId { get; set; }

        public ICollection<StudyParticipant> StudyParticipants { get; set; }
    }
}
