using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Model
{
    public class StudyDataset
    {
        public int StudyId { get; set; }
        public virtual Study Study { get; set; }

        public int DatasetId { get; set; }
        public virtual Dataset Dataset { get; set; }
    }
}
