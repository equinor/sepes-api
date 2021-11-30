using Sepes.Common.Dto.ServiceNow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Service.ServiceNow.Interface
{
    public interface IServiceNowService
    {
        void ReportIncident(ReportIssueDto reportIncidentDto);
    }
}
