using Sepes.Infrastructure.Interface;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Study
{
    public class StudyListItemDto : LookupBaseDto, IHasLogoUrl
    {
        public string Description { get; set; }
        public string Vendor { get; set; }
        public bool Restricted { get; set; }
        public string LogoUrl { get; set; }
    }
}
