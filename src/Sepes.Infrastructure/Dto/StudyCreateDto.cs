using Sepes.Infrastructure.Interface;

namespace Sepes.Infrastructure.Dto
{
    public class StudyCreateDto : IHasLogoUrl
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ResultsAndLearnings { get; set; }

        public string WbsCode { get; set; }

        public string Vendor { get; set; }

        public bool Restricted { get; set; }

        public string LogoUrl { get; set; }

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }

    
        
    }
}
