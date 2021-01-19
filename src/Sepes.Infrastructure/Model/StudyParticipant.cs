using System;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class StudyParticipant : IHasCreatedFields
    {
        public int StudyId { get; set; }
     
        public int UserId { get; set; }

        [MaxLength(64)]
        public string RoleName { get; set; }
     
        public DateTime Created { get; set; }

        [MaxLength(64)]
        public string CreatedBy { get; set; }

        public virtual Study Study { get; set; }
        public virtual User User { get; set; }

    } 
}
