using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Model
{
    public class StudyLogo : UpdateableBaseModel
    {
        public byte[] Content { get; set; }

        public int StudyId { get; set; }
        public virtual Study Study { get; set; }
    }
}
