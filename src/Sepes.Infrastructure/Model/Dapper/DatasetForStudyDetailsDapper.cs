using System.Collections.Generic;

namespace Sepes.Infrastructure.Model
{
    public class DatasetForStudyDetailsDapper
    {
        public int DatasetId { get; set; }
        public string DatasetName { get; set; }

        public int? StudyId { get; set; }

        public List<SandboxForStudyDetailsDapper> Sandboxes { get; set; }
    }
}
