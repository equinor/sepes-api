﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Infrastructure.Service.Interface;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controller
{
    [Route("api/sandbox/")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class SandboxDatasetController : ControllerBase
    {
        readonly ISandboxDatasetService _service;

        public SandboxDatasetController(ISandboxDatasetService service)
        {
            _service = service;
        }

        [HttpGet("{sandboxId}/datasets")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetDatasetsForSandbox(int sandboxId)
        {
            var dataset = await _service.GetAll(sandboxId);
            return new JsonResult(dataset);
        }

        [HttpPut("{sandboxId}/datasets/{datasetId}")]
        public async Task<IActionResult> AddDataSetAsync(int sandboxId, int datasetId)
        {
            var updatedStudy = await _service.Add(sandboxId, datasetId);
            return new JsonResult(updatedStudy);
        }

        [HttpDelete("{sandboxId}/datasets/{datasetId}")]
        public async Task<IActionResult> RemoveDataSetAsync(int sandboxId, int datasetId)
        {
            var updatedStudy = await _service.Remove(sandboxId, datasetId);
            return new JsonResult(updatedStudy);
        } 
    }
}
