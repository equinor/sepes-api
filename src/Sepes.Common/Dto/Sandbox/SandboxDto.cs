using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;

namespace Sepes.Common.Dto.Sandbox
{
    public class SandboxDto : UpdateableBaseDto, IHasCurrentPhase
    {
        public string Name { get; set; }

        public int StudyId { get; set; }

        public string StudyName { get; set; }

        public string Region { get; set; }

        public string TechnicalContactName { get; set; }

        public string TechnicalContactEmail { get; set; }

        public SandboxPhase CurrentPhase { get; set; }        

        public bool Deleted { get; set; }     
    }
}
