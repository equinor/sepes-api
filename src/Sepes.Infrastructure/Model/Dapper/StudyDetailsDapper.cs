namespace Sepes.Infrastructure.Model
{
    public class StudyDetailsDapper : SingleEntityDapperResult
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string WbsCode { get; set; }

        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public string LogoUrl { get; set; }

        public bool WbsCodeValid { get; set; }
    }
}
