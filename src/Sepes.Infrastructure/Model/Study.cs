using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Study : UpdateableBaseModel
    {
        [MaxLength(128)]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [MaxLength(64)]
        public string WbsCode { get; set; }

        public ICollection<DataSet> DataSets { get; set; }

        public ICollection<SandBox> SandBoxes { get; set; }

    }
}
