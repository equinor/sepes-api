using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineService : IVirtualMachineService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IStudyService _studyService;
        readonly ISandboxService _sandboxService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IProvisioningQueueService _workQueue;

        public VirtualMachineService(ILogger<VirtualMachineService> logger, SepesDbContext db, IMapper mapper, IUserService userService, IStudyService studyService, ISandboxService sandboxService, ISandboxResourceService sandboxResourceService, IProvisioningQueueService workQueue)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _studyService = studyService;
            _sandboxService = sandboxService;
            _sandboxResourceService = sandboxResourceService;
            _workQueue = workQueue;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto newVmDto)
        {
            _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");


            var sandbox = await _sandboxService.GetSandbox(sandboxId);
            var study = await _studyService.GetStudyByIdAsync(sandbox.StudyId);
            var vmResourceEntry = await _sandboxResourceService.CreateVmEntryAsync(study, sandbox, newVmDto);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = sandboxId;
            queueParentItem.Description = $"Create VM for Sandbox: {sandboxId}";

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = vmResourceEntry.Operations.FirstOrDefault().Id.Value });

            await _workQueue.SendMessageAsync(queueParentItem);

            return new VmDto();
        }

        public Task<VmDto> UpdateAsync(int sandboxDto, CreateVmUserInputDto newSandbox)
        {
            throw new NotImplementedException();
        }

        public Task<VmDto> DeleteAsync(int id)
        {
            //Remove sandbox resource (mark as deleted)
            //Remember to add to queue
            //Remember to check dependency in queue
            //Also remember to delete osdisk

            throw new NotImplementedException();
        }


        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        }
    }
}
