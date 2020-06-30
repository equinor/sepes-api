using System;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public interface IHasCreatedFields
    {
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }
    }

    public interface IHasUpdatedFields
    {
        public DateTime Updated { get; set; }

        public string UpdatedBy { get; set; }
    }
    public class BaseModel : IHasCreatedFields
    {
        public int Id { get; set; }
     
        public DateTime Created { get; set; }

        [MaxLength(64)]
        public string CreatedBy { get; set; }  
    }

    public class UpdateableBaseModel : BaseModel, IHasUpdatedFields
    {
        public DateTime Updated { get; set; }

        [MaxLength(64)]
        public string UpdatedBy { get; set; }
    }
}
