using System.Collections.Generic;

namespace Sepes.Infrastructure.Model
{
    public class Dataset : UpdateableBaseModel
    { 
        public string Name { get; set; }    

        public ICollection<StudyDataset> StudyDatasets { get; set; }

        //StudyID is only populated if dataset is StudySpecific.
        //This is accounted for in API calls.
        public int? StudyID { get; set; }
    }
}
