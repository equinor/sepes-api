using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.ServiceNow;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.ServiceNow
{
    [Route("api/servicenow")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Create : EndpointBase.WithRequest<ServiceNowEnquiryCreateDto>.WithActionResult
    {
        private readonly IServiceNowApiService _serviceNowApiService;
        public Create(IServiceNowApiService serviceNowApiService)
        {
            _serviceNowApiService = serviceNowApiService;
        }

        [HttpPost]
        public override async Task<ActionResult> Handle(ServiceNowEnquiryCreateDto enquiry, CancellationToken cancellationToken = default)
        {           
            var userNameClaim = User.Claims.SingleOrDefault(c => c.Type == "preferred_username");
            enquiry.CallerId = userNameClaim.Value;
            var response = await _serviceNowApiService.CreateEnquiry(enquiry);
            return new JsonResult(response);
        }
    }
}
