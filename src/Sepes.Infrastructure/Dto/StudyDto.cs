using Sepes.Infrastructure.Interface;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Dto
{
    public class StudyDto : UpdateableBaseDto, IHasLogoUrl
    {
        [MaxLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string ResultsAndLearnings { get; set; }
        [MaxLength(64)]
        public string WbsCode { get; set; }
        [MaxLength(128)]
        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public string LogoUrl { get; set; }

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }       

        public ICollection<DatasetDto> Datasets { get; set; }

        public ICollection<SandboxDto> Sandboxes { get; set; }

        public ICollection<StudyParticipantDto> Participants { get; set; }
        
    }
}
