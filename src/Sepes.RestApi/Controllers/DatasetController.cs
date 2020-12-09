using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/datasets")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class DatasetController : ControllerBase
    {
        readonly IDatasetService _datasetService;
        readonly IDatasetFileService _datasetFileService;
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public DatasetController(IDatasetService datasetService, IDatasetFileService datasetFileService, IStudySpecificDatasetService studySpecificDatasetService)
        {
            _datasetService = datasetService;
            _datasetFileService = datasetFileService;
            _studySpecificDatasetService = studySpecificDatasetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDatasetsAsync()
        {
            var studies = await _datasetService.GetDatasetsAsync();
            return new JsonResult(studies);
        }

        //Get list of datasets lookup items
        [HttpGet("lookup")]
        public async Task<IActionResult> GetDatasetsLookupAsync()
        {
            var studies = await _datasetService.GetDatasetsLookupAsync();
            return new JsonResult(studies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataset(int id)
        {
            var dataset = await _datasetService.GetDatasetByDatasetIdAsync(id);
            return new JsonResult(dataset);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateDataset(DatasetDto newDataset)
        {
            var dataset = await _datasetService.CreateDatasetAsync(newDataset);
            return new JsonResult(dataset);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDataset(int id, DatasetDto dataset)
        {
            var updatedDataset = await _datasetService.UpdateDatasetAsync(id, dataset);
            return new JsonResult(updatedDataset);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataset(int id, CancellationToken cancellationToken = default)
        {
            if (await _datasetService.IsStudySpecific(id))
            {
                await _studySpecificDatasetService.SoftDeleteStudySpecificDatasetAsync(id, cancellationToken);
            }
            else
            {
                await _datasetService.DeleteDatasetAsync(id);
            }
           
            return new NoContentResult();
        }     

        [HttpPost("{datasetId}/file")]
        public async Task<IActionResult> AddFile(int datasetId, [FromForm] IFormFile files, CancellationToken cancellationToken = default)
        {
            var fileAddResult = await _datasetFileService.AddFiles(datasetId, new List<IFormFile> { files }, cancellationToken);
            return new JsonResult(fileAddResult);
        }

        [HttpPost("{datasetId}/files")]
        public async Task<IActionResult> AddFiles(int datasetId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var fileAddResult = await _datasetFileService.AddFiles(datasetId, files, cancellationToken);
            return new JsonResult(fileAddResult);
        }

        [HttpGet("{datasetId}/files")]
        public async Task<IActionResult> GetFileList(int datasetId, CancellationToken cancellationToken = default)
        {
            var files = await _datasetFileService.GetFileList(datasetId, cancellationToken);
            return new JsonResult(files);
        }

        [HttpDelete("{datasetId}/files/fileName")]
        public async Task<IActionResult> DeleteFile(int datasetId, string fileName, CancellationToken cancellationToken = default)
        {
            await _datasetFileService.DeleteFile(datasetId, fileName, cancellationToken);
            return new NoContentResult();
        }
    }
}