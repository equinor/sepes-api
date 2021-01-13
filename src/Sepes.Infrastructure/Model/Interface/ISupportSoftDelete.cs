using System;

namespace Sepes.Infrastructure.Model.Interface
{
    public interface ISupportSoftDelete
    {
        bool? Deleted { get; set; }
        
        string DeletedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
