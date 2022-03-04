using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using Sepes.RestApi.Util;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.StudiesDatasets
{
    [Route("api/studies")]
    public class Create : EndpointBase
    {
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public Create(IStudySpecificDatasetService studySpecificDatasetService)
        {
            _studySpecificDatasetService = studySpecificDatasetService;
        }

        [HttpPost("{studyId}/datasets/studyspecific")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Handle(int studyId, DatasetCreateUpdateInputBaseDto newDataset, CancellationToken cancellation = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);

            var createdDataset = await _studySpecificDatasetService.CreateStudySpecificDatasetAsync(studyId, newDataset, clientIp, cancellation);
            return new JsonResult(createdDataset);
        }
    }
}
