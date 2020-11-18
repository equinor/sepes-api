using System;
using System.ComponentModel.DataAnnotations;

namespace Sepes.Infrastructure.Model
{
    public interface IHasCreatedFields
    {
        DateTime Created { get; set; }

        string CreatedBy { get; set; }
    }

    public interface IHasUpdatedFields
    {
        DateTime Updated { get; set; }

        string UpdatedBy { get; set; }
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

    public class StringKeyBaseModel : IHasCreatedFields
    {
        public string Key { get; set; }

        public DateTime Created { get; set; }

        [MaxLength(64)]
        public string CreatedBy { get; set; }
    }

    public class UpdateableStringKeyBaseModel : StringKeyBaseModel, IHasUpdatedFields
    {
        public DateTime Updated { get; set; }

        [MaxLength(64)]
        public string UpdatedBy { get; set; }
    }
}
