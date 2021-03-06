﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.Controllers
{
    [Route("api/virtualmachines")]
    [ApiController]
    [Produces("application/json")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Authorize]
    public class VirtualMachineController : ControllerBase
    {
        readonly IVirtualMachineCreateService _virtualMachineCreateService;
        readonly IVirtualMachineReadService _virtualMachineReadService;
        readonly IVirtualMachineDeleteService _virtualMachineDeleteService;    

        public VirtualMachineController(
            IVirtualMachineCreateService virtualMachineCreateService,
            IVirtualMachineReadService virtualMachineReadService,
            IVirtualMachineDeleteService virtualMachineDeleteService)
        {
            _virtualMachineCreateService = virtualMachineCreateService;
            _virtualMachineReadService = virtualMachineReadService;
            _virtualMachineDeleteService = virtualMachineDeleteService;          
        }

        [HttpPost("{sandboxId}")]
        public async Task<IActionResult> CreateAsync(int sandboxId, VirtualMachineCreateDto newVm)
        {
            var createdVm = await _virtualMachineCreateService.CreateAsync(sandboxId, newVm);
            return new JsonResult(createdVm);
        }

        [HttpDelete("{vmId}")]
        public async Task<IActionResult> DeleteAsync(int vmId)
        {
            await _virtualMachineDeleteService.DeleteAsync(vmId);
            return new NoContentResult();
        }

        [HttpGet("forsandbox/{sandboxId}")]
        public async Task<IActionResult> GetAllVMsForSandbox(int sandboxId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.VirtualMachinesForSandboxAsync(sandboxId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/extended")]
        public async Task<IActionResult> GetVmExtendedInfo(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.GetExtendedInfo(vmId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }

        [HttpGet("{vmId}/externalLink")]
        public async Task<IActionResult> GetVmExternalLink(int vmId, CancellationToken cancellationToken = default)
        {
            var virtualMachinesForSandbox = await _virtualMachineReadService.GetExternalLink(vmId, cancellationToken);
            return new JsonResult(virtualMachinesForSandbox);
        }     
    }
}
