using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.ServiceNow;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/servicenow")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class ServiceNowController
    {
        private readonly IServiceNowApiService _serviceNowApiService;
        public ServiceNowController(IServiceNowApiService serviceNowApiService)
        {
            _serviceNowApiService = serviceNowApiService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceNowEnquiryCreateDto enquiry)
        {
            var response = await _serviceNowApiService.CreateEnquiry(enquiry);
            return new JsonResult(response);
        }
    }
}
