using Sepes.Common.Dto.Study;
using System.Collections.Generic;

namespace Sepes.Common.Dto.Interfaces
{
    public interface IHasStudyParticipants
    {
        List<StudyParticipantDto> Participants { get; set; }
    }
}
