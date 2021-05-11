using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Interface;

namespace Sepes.Common.Dto.Study
{
    public class StudyCreateDto
    {
        public string Name { get; set; }

        public string Description { get; set; }     

        public string WbsCode { get; set; }

        public string Vendor { get; set; }

        public bool Restricted { get; set; }
    }
}
