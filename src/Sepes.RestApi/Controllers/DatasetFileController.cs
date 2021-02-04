using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
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
        readonly IAzureBlobStorageService _azureBlobStorageService;

        public DatasetFileController(IDatasetFileService datasetFileService)
        {
            _datasetFileService = datasetFileService;
        }      

        [HttpPost("file")]
        public async Task<IActionResult> AddFile(int datasetId, [FromForm] IFormFile files, CancellationToken cancellationToken = default)
        {
            var fileAddResult = await _datasetFileService.AddFiles(datasetId, new List<IFormFile> { files }, cancellationToken);
            return new JsonResult(fileAddResult);
        }

        [HttpPost("files")]
        public async Task<IActionResult> AddFiles(int datasetId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var fileAddResult = await _datasetFileService.AddFiles(datasetId, files, cancellationToken);
            return new JsonResult(fileAddResult);
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFileList(int datasetId, CancellationToken cancellationToken = default)
        {
            var files = await _datasetFileService.GetFileList(datasetId, cancellationToken);
            return new JsonResult(files);
        }

        [HttpDelete("files/fileName")]
        public async Task<IActionResult> DeleteFile(int datasetId, string fileName, CancellationToken cancellationToken = default)
        {
            await _datasetFileService.DeleteFile(datasetId, fileName, cancellationToken);
            return new NoContentResult();
        }
        

        [HttpGet("saskey")]
        public async Task<IActionResult> GetSasToken(int datasetId, CancellationToken cancellationToken = default)
        {
            var sasToken = await _datasetFileService.GetSasToken(datasetId);
            var token = await _azureBlobStorageService.;
            return new JsonResult(sasToken);
        }
    }
}