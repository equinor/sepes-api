using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxDto : UpdateableBaseDto
    {
        public string Name { get; set; }

        public int StudyId { get; set; }

        public string StudyName { get; set; }

        public string Region { get; set; }

        public string TechnicalContactName { get; set; }

        public string TechnicalContactEmail { get; set; }
        public string LinkToCostAnalysis { get; set; }

        public bool Deleted { get; set; }

        public List<SandboxResourceLightDto> Resources { get; set; }

        public List<SandboxDatasetDto> Datasets { get; set; }

        public SandboxPermissionsDto Permissions { get; set; } = new SandboxPermissionsDto();
    }
}
