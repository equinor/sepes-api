using AutoMapper;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineService : IVirtualMachineService
    {
        readonly ILogger _logger;
        readonly IConfiguration _config;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IStudyService _studyService;
        readonly ISandboxService _sandboxService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IProvisioningQueueService _workQueue;

        public VirtualMachineService(ILogger<VirtualMachineService> logger, IConfiguration config, SepesDbContext db, IMapper mapper, IUserService userService, IStudyService studyService, ISandboxService sandboxService, ISandboxResourceService sandboxResourceService, IProvisioningQueueService workQueue)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _mapper = mapper;
            _userService = userService;
            _studyService = studyService;
            _sandboxService = sandboxService;
            _sandboxResourceService = sandboxResourceService;
            _workQueue = workQueue;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto userInput)
        {
            _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");

            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);
            var study = await _studyService.GetStudyByIdAsync(sandbox.StudyId);

            var virtualMachineName = AzureResourceNameUtil.VirtualMachine(study.Name, sandbox.Name, userInput.Name);

            var tags = AzureResourceTagsFactory.CreateTags(_config, study, sandbox);

            var region = RegionStringConverter.Convert(userInput.Region);

            var vmSettingsString = await CreateVmSettingsString(study.Id.Value, sandboxId, userInput);

            var resourceGroup = await SandboxResourceQueries.GetResourceGroupEntry(_db, sandboxId);

            //Make this dependent on bastion create operation to be completed, since bastion finishes last
            var dependsOn = await SandboxResourceQueries.GetCreateOperationIdForBastion(_db, sandboxId);

            var vmResourceEntry = await _sandboxResourceService.CreateVmEntryAsync(sandboxId, resourceGroup, region, tags, virtualMachineName, dependsOn, vmSettingsString);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = sandboxId;
            queueParentItem.Description = $"Create VM for Sandbox: {sandboxId}";

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = vmResourceEntry.Operations.FirstOrDefault().Id.Value });

            await _workQueue.SendMessageAsync(queueParentItem);

            var dtoMappedFromResource = _mapper.Map<VmDto>(vmResourceEntry);

            return dtoMappedFromResource;
        }

        async Task<string> CreateVmSettingsString(int studyId, int sandboxId, CreateVmUserInputDto userInput)
        {
            var vmSettings = _mapper.Map<VmSettingsDto>(userInput);

            vmSettings.Password = await StoreNewVmPasswordAsKeyVaultSecretAndReturnReference(studyId, sandboxId, vmSettings.Password);

            var diagStorageResource = await SandboxResourceQueries.GetDiagStorageAccountEntry(_db, sandboxId);
            vmSettings.DiagnosticStorageAccountName = diagStorageResource.ResourceName;

            var networkResource = await SandboxResourceQueries.GetNetworkEntry(_db, sandboxId);
            vmSettings.NetworkName = networkResource.ResourceName;

            var networkSetting = SandboxResourceConfigStringSerializer.NetworkSettings(networkResource.ConfigString);
            vmSettings.SubnetName = networkSetting.SandboxSubnetName;

            return SandboxResourceConfigStringSerializer.Serialize(vmSettings);
        }

        async Task<string> StoreNewVmPasswordAsKeyVaultSecretAndReturnReference(int studyId, int sandboxId, string password)
        {
            var keyVaultSecretName = $"newvmpassword-{studyId}-{sandboxId}-{Guid.NewGuid().ToString().Replace("-", "")}";

            await KeyVaultSecretUtil.AddKeyVaultSecret(_logger, _config, ConfigConstants.AZURE_VM_TEMP_PASSWORD_KEY_VAULT, keyVaultSecretName, password);

            return keyVaultSecretName;
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

        public async Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId)
        {
            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            var virtualMachines = await SandboxResourceQueries.GetSandboxVirtualMachinesList(_db, sandbox.Id.Value);

            return _mapper.Map<List<VmDto>>(virtualMachines);
        }
    }
}
