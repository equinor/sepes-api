using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/participants")]
    [ApiController]
    [Authorize]
    public class ParticipantController : ControllerBase
    {
        readonly IParticipantService _participantService;

        public ParticipantController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        //Get list of lookup items
        [HttpGet]
        public async Task<IActionResult> GetLookupAsync()
        {
            var studies = await _participantService.GetLookupAsync();
            return new JsonResult(studies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var participant = await _participantService.GetByIdAsync(id);
            return new JsonResult(participant);
        }       
    }

}
