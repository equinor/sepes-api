﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints
{
    [Route("api/permissions")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Get : ControllerBase
    {
        readonly IUserPermissionService _service;

        public Get(IUserPermissionService userService)
        {
            _service = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Handle()
        {
            var permissions = await _service.GetUserPermissionsAsync();
            return new JsonResult(permissions);
        }
    }
}
