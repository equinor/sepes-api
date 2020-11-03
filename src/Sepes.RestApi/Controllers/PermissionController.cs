using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize] //Todo: Need wider access, but restricted for now
    public class PermissionController : ControllerBase
    {
        readonly IUserPermissionService _service;

        public PermissionController(IUserPermissionService userService)
        {
            _service = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
          var permissions = await _service.GetUserPermissionsAsync();

            return new JsonResult(permissions);
        }
    }
}
