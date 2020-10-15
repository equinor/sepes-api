using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.RestApi.Controllers
{
    [Route("api/permissions")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize] //Todo: Need wider access, but restricted for now
    public class PermissionController : ControllerBase
    {
        readonly IUserService _userService;

        public PermissionController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions()
        {
          var permissions = await _userService.GetUserPermissionsAsync();

            return new JsonResult(permissions);
        }
    }
}
