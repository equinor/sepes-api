using Sepes.Infrastructure.Interface;

namespace Sepes.Infrastructure.Response
{
    public class StudyListItemResponse : IHasLogoUrl
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string Vendor { get; set; }
        public bool Restricted { get; set; }
        public string LogoUrl { get; set; }
    }
}
