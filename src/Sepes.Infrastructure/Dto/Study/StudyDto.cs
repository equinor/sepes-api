using Sepes.Infrastructure.Dto.Interfaces;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Study
{
    public class StudyDto : UpdateableBaseDto, IHasStudyParticipants
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string WbsCode { get; set; }

        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public string LogoUrl { get; set; }

        public List<StudyParticipantDto> Participants { get; set; }        
    }
}
