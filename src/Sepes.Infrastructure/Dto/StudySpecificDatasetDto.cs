using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto
{
    public class StudySpecificDatasetDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public int StudyId { get; set; }
    }
}
