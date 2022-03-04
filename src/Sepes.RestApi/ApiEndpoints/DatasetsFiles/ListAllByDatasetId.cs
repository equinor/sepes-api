using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using Sepes.RestApi.Util;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.DatasetsFiles
{
    [Route("api/datasets/{datasetId}")]
    public class ListAllByDatasetId : EndpointBase
    {
        readonly IDatasetFileService _datasetFileService;

        public ListAllByDatasetId(IDatasetFileService datasetFileService)
        {
            _datasetFileService = datasetFileService;
        }

        [HttpGet("files")]
        public async Task<IActionResult> Handle(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var files = await _datasetFileService.GetFileListAsync(datasetId, clientIp, cancellationToken);
            return new JsonResult(files);
        }
    }
}
