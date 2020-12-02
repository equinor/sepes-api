namespace Sepes.Infrastructure.Dto.Study
{
    public class StudyPermissionsDto
    {  
        public bool UpdateMetadata { get; set; }

        public bool ReadResulsAndLearnings { get; set; }

        public bool UpdateResulsAndLearnings { get; set; }

        public bool CloseStudy { get; set; }
        public bool DeleteStudy { get; set; }

        public bool AddRemoveDataset { get; set; }

        public bool AddRemoveParticipant { get; set; }

        public bool AddRemoveSandbox { get; set; }
    }
}
