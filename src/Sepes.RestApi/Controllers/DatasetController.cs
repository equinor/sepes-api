﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Service.DataModelService.Interface;
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
        readonly IPreApprovedDatasetModelService _preApprovedDatasetModelService;

        public DatasetController(IDatasetService datasetService, IStudySpecificDatasetService studySpecificDatasetService, IPreApprovedDatasetModelService preApprovedDatasetModelService)
        {
            _datasetService = datasetService;
            _studySpecificDatasetService = studySpecificDatasetService;
            _preApprovedDatasetModelService = preApprovedDatasetModelService;
        }

        //UNSUPPORTED METHODS COMMENTED OUT TO DECREASE POTENTIAL AREA OF ATTACK. THIS FUNCTIONALITY IS NOT YET FULLE SPECIFIED, SO IT DOES NOT MAKE ANY SENSE TO WRITE TESTS FOR IT YET
        //[HttpGet]
        //public async Task<IActionResult> GetAllAsync()
        //{
        //    var studies = await _datasetService.GetAllAsync();
        //    return new JsonResult(studies);
        //}

       
        //[HttpGet("lookup")]
        //public async Task<IActionResult> GetLookupAsync()
        //{
        //    var studies = await _datasetService.GetLookupAsync();
        //    return new JsonResult(studies);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetByIdAsync(int id)
        //{
        //    var dataset = await _datasetService.GetByIdAsync(id);
        //    return new JsonResult(dataset);
        //}

        //[HttpPost("")]
        //public async Task<IActionResult> CreateAsync(PreApprovedDatasetCreateUpdateDto newDataset)
        //{
        //    var dataset = await _datasetService.CreateAsync(newDataset);
        //    return new JsonResult(dataset);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateAsync(int id, DatasetDto dataset)
        //{
        //    var updatedDataset = await _datasetService.UpdateAsync(id, dataset);
        //    return new JsonResult(updatedDataset);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
        //{
        //    await _datasetService.DeleteAsync(id);

        //    return new NoContentResult();
        //}     
    }
}