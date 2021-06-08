using System.Collections.Generic;

namespace Sepes.Infrastructure.Model
{
    public class StudyParticipantForStudyDetailsDapper
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }

        public int StudyId { get; set; }
    }
}
