using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudyResultsAndLearnings
{
    [Route("api/studies")]
    public class Get : EndpointBase
    {
        readonly IStudyResultsAndLearningsModelService _studyResultsAndLearningsModelService;

        public Get(IStudyResultsAndLearningsModelService studyResultsAndLearningsModelService)
        {
            _studyResultsAndLearningsModelService = studyResultsAndLearningsModelService;
        }

        [HttpGet("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int studyId)
        {
            var resultsAndLearnings = await _studyResultsAndLearningsModelService.GetAsync(studyId);
            return new JsonResult(resultsAndLearnings);
        }
    }
}
