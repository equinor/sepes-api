using System;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public class BaseModel
    {
        public int Id { get; set; }
     
        public DateTime Created { get; set; }

        [MaxLength(64)]
        public string CreatedBy { get; set; }  
    }

    public class UpdateableBaseModel : BaseModel
    {
        public DateTime Updated { get; set; }

        [MaxLength(64)]
        public string UpdatedBy { get; set; }
    }
}
