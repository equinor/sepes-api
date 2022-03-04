using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.ApiEndpoints.Base;
using Sepes.RestApi.Util;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.ApiEndpoints.DatasetsFiles
{
    [Route("api/datasets/{datasetId}")]
    public class GetFileDeleteSasToken : EndpointBase
    {
        readonly IDatasetFileService _datasetFileService;

        public GetFileDeleteSasToken(IDatasetFileService datasetFileService)
        {
            _datasetFileService = datasetFileService;
        }

        [HttpGet("saskeydelete")]
        public async Task<IActionResult> Handle(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var sasTokenDelete = await _datasetFileService.GetFileDeleteUriAsync(datasetId, clientIp, cancellationToken);
            return new JsonResult(sasTokenDelete);
        }
    }
}
