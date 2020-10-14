using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class SandboxDto : UpdateableBaseDto
    {
        public string Name { get; set; }

        public int StudyId { get; set; }

        public string StudyName { get; set; }

        public string Region { get; set; }

        public string TechnicalContactName { get; set; }

        public string TechnicalContactEmail { get; set; }

        public bool Deleted { get; set; }

        public List<SandboxResourceLightDto> Resources { get; set; }


    }
}
