using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;
using Sepes.RestApi.Util;

namespace Sepes.RestApi.Controller
{
    [Route("api/datasets/{datasetId}")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class DatasetFileController : ControllerBase
    {
        readonly IDatasetFileService _datasetFileService;     

        public DatasetFileController(IDatasetFileService datasetFileService)
        {
            _datasetFileService = datasetFileService;
        }        

        [HttpGet("files")]
        public async Task<IActionResult> GetFileList(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var files = await _datasetFileService.GetFileListAsync(datasetId, clientIp, cancellationToken);
            return new JsonResult(files);
        }              

        [HttpGet("saskey")]
        public async Task<IActionResult> GetFileUploadSasToken(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var sasToken = await _datasetFileService.GetFileUploadUriAsync(datasetId, clientIp, cancellationToken);         
            return new JsonResult(sasToken);
        }

        [HttpGet("saskeydelete")]
        public async Task<IActionResult> GetFileDeleteSasToken(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var sasTokenDelete = await _datasetFileService.GetFileDeleteUriAsync(datasetId, clientIp, cancellationToken);
            return new JsonResult(sasTokenDelete);
        }
    }
}