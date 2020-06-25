namespace Sepes.Infrastructure.Model
{
    public class StudyParticipant
    {
        public int StudyId { get; set; }
        public virtual Study Study { get; set; }
        public int ParticipantId { get; set; }
        public virtual Participant Participant { get; set; }
    }
}
