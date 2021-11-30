using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.ServiceNow;
using Sepes.Infrastructure.Service.ServiceNow.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi
{
    [Route("api/reportissue")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class ServicenowController : ControllerBase
    {
        readonly IServiceNowService _serviceNowService;
        public ServicenowController (IServiceNowService serviceNowService)
        {
            _serviceNowService = serviceNowService;
        }

        [HttpPost]
        public async Task<IActionResult> ReportIssue( ReportIssueDto reportIssueDto)
        {
            //var studies = await _datasetService.GetAllAsync();
            //return new JsonResult(studies);
            _serviceNowService.ReportIncident(reportIssueDto);
            return Ok();
        }
    }
}
