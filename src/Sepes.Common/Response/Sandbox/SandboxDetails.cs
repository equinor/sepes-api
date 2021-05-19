using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using System.Collections.Generic;
using Sepes.Common.Interface;
using Sepes.Common.Model;

namespace Sepes.Common.Response.Sandbox
{
    public class SandboxDetails : UpdateableBaseDto, IHasCurrentPhase
    {
        public string Name { get; set; }

        public int StudyId { get; set; }

        public string StudyName { get; set; }

        public string Region { get; set; }

        public string TechnicalContactName { get; set; }

        public string TechnicalContactEmail { get; set; }
        public string LinkToCostAnalysis { get; set; }

        public SandboxPhase CurrentPhase { get; set; }

        public bool Deleted { get; set; }   

        public string RestrictionDisplayText { get; set; }

        public List<SandboxDatasetDto> Datasets { get; set; }

        public SandboxPermissions Permissions { get; set; } = new SandboxPermissions();
    }
}
