using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class StudyListItemDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public bool Restricted { get; set; }
        public string LogoUrl { get; set; }
    }
}
