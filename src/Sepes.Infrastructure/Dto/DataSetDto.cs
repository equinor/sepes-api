using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class DatasetDto : UpdateableBaseDto
    {
        public string Name { get; set; }

        public ICollection<StudyDto> Studies { get; set; }
    }
}
