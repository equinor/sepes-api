using System.Collections.Generic;

namespace Sepes.Infrastructure.Model
{
    public class Dataset : UpdateableBaseModel
    { 
        public string Name { get; set; }    

        public ICollection<StudyDataset> StudyDatasets { get; set; }

        public int? StudyID { get; set; }
    }
}
