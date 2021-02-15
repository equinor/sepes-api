using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        [HttpPost("file")]
        public async Task<IActionResult> AddFile(int datasetId, [FromForm] IFormFile files, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var fileAddResult = await _datasetFileService.AddFiles(datasetId, new List<IFormFile> { files }, clientIp, cancellationToken);
            return new JsonResult(fileAddResult);
        }

        [HttpPost("files")]
        public async Task<IActionResult> AddFiles(int datasetId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var fileAddResult = await _datasetFileService.AddFiles(datasetId, files, clientIp, cancellationToken);
            return new JsonResult(fileAddResult);
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFileList(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var files = await _datasetFileService.GetFileListAsync(datasetId, clientIp, cancellationToken);
            return new JsonResult(files);
        }

        [HttpDelete("files/fileName")]
        public async Task<IActionResult> DeleteFile(int datasetId, string fileName, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            await _datasetFileService.DeleteFileAsync(datasetId, fileName, clientIp, cancellationToken);
            return new NoContentResult();
        }        

        [HttpGet("saskey")]
        public async Task<IActionResult> GetFileUploadSasToken(int datasetId, CancellationToken cancellationToken = default)
        {
            var clientIp = IpAddressUtil.GetClientIp(HttpContext);
            var sasToken = await _datasetFileService.GetFileUploadUriBuilderWithSasTokenAsync(datasetId, clientIp, cancellationToken);         
            return new JsonResult(sasToken);
        }
    }
}