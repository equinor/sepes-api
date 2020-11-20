namespace Sepes.Infrastructure.Dto.Study
{
    public class StudyPermissionsDto
    {  
        public bool UpdateDetails { get; set; }

        public bool Delete { get; set; }

        public bool AddRemoveDataset { get; set; }

        public bool AddRemoveParticipant { get; set; }

        public bool AddRemoveSandbox { get; set; }

        public bool NavigateToSandbox { get; set; }

        public bool NavigateToDataset { get; set; }
    }
}
