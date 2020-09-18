using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("azureadusers")]
    [Authorize]
    public class AzureADUserController : ControllerBase
    {
        private readonly IAzureADUsersService _azureADUsersService;

        public AzureADUserController(IAzureADUsersService azureADUsersService)
        {
            _azureADUsersService = azureADUsersService;
        }

        [HttpGet("searchusers")]
        [ProducesResponseType(typeof(AzureADUserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUsers(string search, int limit)
        {
            var result = await _azureADUsersService.SearchUsers(search, limit);
            return Ok(result);
        }


    }
}
