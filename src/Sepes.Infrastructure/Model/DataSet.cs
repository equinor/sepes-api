using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class DataSet : UpdateableBaseModel
    { 
        public string Name { get; set; }

        public int StudyId { get; set; }

        public Study Study { get; set; }
    }
}
