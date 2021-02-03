using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Service.Interface;
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
        readonly IStudySpecificDatasetService _studySpecificDatasetService;

        public DatasetController(IDatasetService datasetService, IStudySpecificDatasetService studySpecificDatasetService)
        {
            _datasetService = datasetService;
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
        public async Task<IActionResult> CreateDataset(PreApprovedDatasetCreateUpdateDto newDataset)
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
                await _studySpecificDatasetService.HardDeleteStudySpecificDatasetAsync(id, cancellationToken);
            }
            else
            {
                await _datasetService.DeleteDatasetAsync(id);
            }
           
            return new NoContentResult();
        }     
    }
}