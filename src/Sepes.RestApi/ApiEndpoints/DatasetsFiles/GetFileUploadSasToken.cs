using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using Sepes.RestApi.Util;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.DatasetsFiles
{
    [Route("api/datasets/{datasetId}")]
    public class GetFileUploadSasToken : EndpointBase
    {
        readonly IDatasetFileService _datasetFileService;
        public GetFileUploadSasToken(IDatasetFileService datasetFileService)
        {
            _datasetFileService = datasetFileService;
        }

        [HttpGet("saskey")]
        public async Task<IActionResult> Handle(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var sasToken = await _datasetFileService.GetFileUploadUriAsync(datasetId, clientIp, cancellationToken);
            return new JsonResult(sasToken);
        }
    }
}
