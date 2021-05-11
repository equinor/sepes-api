using System;

namespace Sepes.Common.Dto
{
    public class BaseDto
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }  
    }

    public class UpdateableBaseDto : BaseDto
    {
        public DateTime Updated { get; set; }

        public string UpdatedBy { get; set; }
    }

    public class LookupBaseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
