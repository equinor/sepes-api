using System;

namespace Sepes.Infrastructure.Model
{
    public class StudyParticipant : IHasCreatedFields
    {
        public int StudyId { get; set; }
     
        public int ParticipantId { get; set; }
        public string RoleName { get; set; }
     
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }

        public virtual Study Study { get; set; }
        public virtual Participant Participant { get; set; }

    } 
}
