using Sepes.Infrastructure.Dto.Study;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Interfaces
{
    public interface IHasStudyParticipants
    {
        List<StudyParticipantDto> Participants { get; set; }
    }
}
