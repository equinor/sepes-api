using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
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

        public DatasetController(IDatasetService datasetService)
        {
            _datasetService = datasetService;
        }

        //Get list of datasets lookup items
        [HttpGet]
        public async Task<IActionResult> GetDatasetsLookupAsync()
        {
            var studies = await _datasetService.GetDatasetsLookupAsync();
            return new JsonResult(studies);          
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataset(int id)
        {
            var dataset = await _datasetService.GetDatasetByIdAsync(id);
            return new JsonResult(dataset);        
        }

        //[HttpPost()]
        //public async Task<IActionResult> CreateDataset(DatasetDto newDataset)
        //{
        //    var study = await _studyService.CreateDatasetAsync(newDataset);
        //    return new JsonResult(dataset);
        //}


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateDataset(int id, DatasetDto dataset)
        //{
        //    var updatedDataset = await _studyService.UpdateDatasetAsync(id, dataset);
        //    return new JsonResult(updatedDataset);
        //}


      


    }

}
