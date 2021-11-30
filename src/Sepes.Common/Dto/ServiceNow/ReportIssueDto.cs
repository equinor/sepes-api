using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Dto.ServiceNow
{
    public class ReportIssueDto
    {
        public string Subject { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
