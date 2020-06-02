using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class Study : UpdateableBaseModel
    {
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(64)]
        public string WbsCode { get; set; }

        //[MaxLength(4000)]
        //public string JsonData { get; set; } 
    }
}
