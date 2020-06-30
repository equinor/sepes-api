using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class StudyDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string WbsCode { get; set; }

        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public string LogoUrl { get; set; }

        public ICollection<DatasetDto> Datasets { get; set; }

        public ICollection<SandboxDto> Sandboxes { get; set; }

        public ICollection<ParticipantDto> Participants { get; set; }
        
    }
}
