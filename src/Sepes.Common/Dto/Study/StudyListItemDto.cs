using Sepes.Infrastructure.Interface;

namespace Sepes.Common.Dto.Study
{
    public class StudyListItemDto : LookupBaseDto, IHasLogoUrl
    {
        public int StudyId { get { return Id; } set { Id = value; } }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public bool Restricted { get; set; }
        public string LogoUrl { get; set; }
    }
}
