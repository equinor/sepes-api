using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/datasets")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize(Roles = Roles.Admin)] //Todo: Need wider access, but keeping it restricted for now
    public class DatasetController : ControllerBase
    {     
        readonly IDatasetService _datasetService;

        public DatasetController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
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
    }

}
