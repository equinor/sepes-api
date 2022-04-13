using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.Studies
{
    [Route("api/studies")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class Close : ControllerBase
    {
        readonly IStudyDeleteService _studyDeleteService;

        public Close(IStudyDeleteService studyDeleteService)
        {
            _studyDeleteService = studyDeleteService;
        }

        [HttpPut("{studyId}/close")]
        public async Task<IActionResult> Handle(int studyId, [FromBody] StudyCloseRequest request)
        {
            bool deleteResources;
            if (request == null)
                deleteResources = true;
            else
                deleteResources = request.DeleteResources;

            await _studyDeleteService.CloseStudyAsync(studyId, deleteResources);
            return new NoContentResult();
        }
    }

    public class StudyCloseRequest
    {
        public bool DeleteResources { get; set; }
    }
}
