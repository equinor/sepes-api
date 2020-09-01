namespace Sepes.Infrastructure.Dto
{
    public class SandboxDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public int StudyId { get; set; }

        public string Region { get; set; }

        public string TechnicalContactName { get; set; }

        public string TechnicalContactEmail { get; set; }

        public bool Deleted { get; set; }


    }
}
