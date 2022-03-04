using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Handlers.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudyResultsAndLearnings
{
    [Route("api/studies")]
    public class Update : EndpointBase
    {
        readonly IStudyResultsAndLearningsUpdateHandler _studyResultsAndLearningsUpdateHandler;

        public Update(IStudyResultsAndLearningsUpdateHandler studyResultsAndLearningsUpdateHandler)
        {
            _studyResultsAndLearningsUpdateHandler = studyResultsAndLearningsUpdateHandler;
        }

        [HttpPut("{studyId}/resultsandlearnings")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateResultsAndLearningsAsync(int studyId, StudyResultsAndLearningsDto resultsAndLearnings)
        {
            var resultsAndLearningsFromDb = await _studyResultsAndLearningsUpdateHandler.UpdateAsync(studyId, resultsAndLearnings);
            return new JsonResult(resultsAndLearningsFromDb);
        }
    }
}
