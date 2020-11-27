using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        readonly IVirtualMachineSizeService _vmSizeService;
        readonly IVirtualMachineLookupService _vmLookupService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IProvisioningQueueService _workQueue;
        readonly IAzureVmService _azureVmService;


        public VirtualMachineService(ILogger<VirtualMachineService> logger,
            IConfiguration config,
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IStudyService studyService,
            ISandboxService sandboxService,
            IVirtualMachineSizeService vmSizeService,
            IVirtualMachineLookupService vmLookupService,
            ISandboxResourceService sandboxResourceService,
            IProvisioningQueueService workQueue,
            IAzureVmService azureVmService)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _mapper = mapper;
            _userService = userService;
            _studyService = studyService;
            _sandboxService = sandboxService;
            _vmSizeService = vmSizeService;
            _vmLookupService = vmLookupService;
            _sandboxResourceService = sandboxResourceService;
            _workQueue = workQueue;
            _azureVmService = azureVmService;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto userInput)
        {
            _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");

            var sandbox = await _sandboxService.GetAsync(sandboxId, UserOperation.Study_Crud_Sandbox);
            var study = await _studyService.GetStudyDtoByIdAsync(sandbox.StudyId, UserOperation.Study_Crud_Sandbox);

            var virtualMachineName = AzureResourceNameUtil.VirtualMachine(study.Name, sandbox.Name, userInput.Name);

            await _sandboxResourceService.ValidateNameThrowIfInvalid(virtualMachineName);

            var tags = AzureResourceTagsFactory.CreateTags(_config, study, sandbox);

            var region = RegionStringConverter.Convert(sandbox.Region);

            var resourceGroup = await SandboxResourceQueries.GetResourceGroupEntry(_db, sandboxId);

            //Make this dependent on bastion create operation to be completed, since bastion finishes last
            var dependsOn = await SandboxResourceQueries.GetCreateOperationIdForBastion(_db, sandboxId);

            var vmResourceEntry = await _sandboxResourceService.CreateVmEntryAsync(sandboxId, resourceGroup, region, tags, virtualMachineName, dependsOn, null);

            //Create vm settings and immeately attach to resource entry
            var vmSettingsString = await CreateVmSettingsString(sandbox.Region, vmResourceEntry.Id, study.Id, sandboxId, userInput);
            vmResourceEntry.ConfigString = vmSettingsString;
            await _sandboxResourceService.Update(vmResourceEntry.Id, vmResourceEntry);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = sandboxId;
            queueParentItem.Description = $"Create VM for Sandbox: {sandboxId}";

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = vmResourceEntry.Operations.FirstOrDefault().Id });

            await _workQueue.SendMessageAsync(queueParentItem);

            var dtoMappedFromResource = _mapper.Map<VmDto>(vmResourceEntry);

            return dtoMappedFromResource;
        }

        public Task<VmDto> UpdateAsync(int sandboxDto, CreateVmUserInputDto newSandbox)
        {
            throw new NotImplementedException();
        }

        public async Task<VmDto> DeleteAsync(int id)
        {
            _ = await GetVmResourceEntry(id, UserOperation.Study_Crud_Sandbox);

            var deletedResource = await _sandboxResourceService.MarkAsDeletedAndScheduleDeletion(id);

            var dtoMappedFromResource = _mapper.Map<VmDto>(deletedResource);

            return dtoMappedFromResource;
        }

        public async Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default)
        {
            var sandbox = await _sandboxService.GetAsync(sandboxId, UserOperation.Study_Read);

            var virtualMachines = await SandboxResourceQueries.GetSandboxVirtualMachinesList(_db, sandbox.Id);

            var virtualMachinesMapped = _mapper.Map<List<VmDto>>(virtualMachines);

            return virtualMachinesMapped;
        }

        public async Task<VmExtendedDto> GetExtendedInfo(int vmId, CancellationToken cancellationToken = default)
        {

            var vmResource = await GetVmResourceEntry(vmId, UserOperation.Study_Read);

            if (vmResource == null)
            {
                throw NotFoundException.CreateForSandboxResource(vmId);
            }

            var dto = await _azureVmService.GetExtendedInfo(vmResource.ResourceGroupName, vmResource.ResourceName);

            var availableSizes = await _vmSizeService.AvailableSizes(vmResource.Region, cancellationToken);

            var availableSizesDict = availableSizes.ToDictionary(s => s.Key, s => s);

            VmSize curSize = null;

            if (availableSizesDict.TryGetValue(dto.SizeName, out curSize))
            {
                dto.Size = _mapper.Map<VmSizeDto>(curSize);
            }

            return dto;
        }

        public async Task<VmExternalLink> GetExternalLink(int vmId)
        {
            var vmResource = await GetVmResourceEntry(vmId, UserOperation.Study_Read);
            var vmExternalLink = new VmExternalLink();
            vmExternalLink.LinkToExternalSystem = AzureResourceUtil.CreateResourceLink(_config, vmResource);
            vmExternalLink.Id = vmId;

            return vmExternalLink;
        }

        async Task<SandboxResource> GetVmResourceEntry(int vmId, UserOperation operation)
        {
            _ = await StudySingularQueries.GetStudyByResourceIdCheckAccessOrThrow(_db, _userService, vmId, operation);
            var vmResource = await _sandboxResourceService.GetByIdAsync(vmId);

            return vmResource;
        } 
        
        async Task<string> CreateVmSettingsString(string region, int vmId, int studyId, int sandboxId, CreateVmUserInputDto userInput)
        {
            var vmSettings = _mapper.Map<VmSettingsDto>(userInput);

            var availableOs = await _vmLookupService.AvailableOperatingSystems(region);
            vmSettings.OperatingSystemCategory = AzureVmUtil.GetOsCategory(availableOs, vmSettings.OperatingSystem);

            vmSettings.Password = await StoreNewVmPasswordAsKeyVaultSecretAndReturnReference(studyId, sandboxId, vmSettings.Password);

            var diagStorageResource = await SandboxResourceQueries.GetDiagStorageAccountEntry(_db, sandboxId);
            vmSettings.DiagnosticStorageAccountName = diagStorageResource.ResourceName;

            var networkResource = await SandboxResourceQueries.GetNetworkEntry(_db, sandboxId);
            vmSettings.NetworkName = networkResource.ResourceName;

            var networkSetting = SandboxResourceConfigStringSerializer.NetworkSettings(networkResource.ConfigString);
            vmSettings.SubnetName = networkSetting.SandboxSubnetName;

            vmSettings.Rules = AzureVmConstants.RulePresets.CreateInitialVmRules(vmId);
            return SandboxResourceConfigStringSerializer.Serialize(vmSettings);
        }

        async Task<string> StoreNewVmPasswordAsKeyVaultSecretAndReturnReference(int studyId, int sandboxId, string password)
        {
            try
            {
                var keyVaultSecretName = $"newvmpassword-{studyId}-{sandboxId}-{Guid.NewGuid().ToString().Replace("-", "")}";

                await KeyVaultSecretUtil.AddKeyVaultSecret(_logger, _config, ConfigConstants.AZURE_VM_TEMP_PASSWORD_KEY_VAULT, keyVaultSecretName, password);

                return keyVaultSecretName;
            }
            catch (Exception ex)
            {

                throw new Exception($"VM Creation failed. Unable to store VM password in Key Vault. See inner exception for details.", ex);
            }
        }


    }
}
