using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        readonly ISandboxService _sandboxService;
        readonly IVirtualMachineSizeService _vmSizeService;
        readonly IVirtualMachineLookupService _vmLookupService;
        readonly ICloudResourceReadService _sandboxResourceService;
        readonly ICloudResourceCreateService _sandboxResourceCreateService;
        readonly ICloudResourceUpdateService _sandboxResourceUpdateService;
        readonly ICloudResourceDeleteService _sandboxResourceDeleteService;
        readonly IProvisioningQueueService _workQueue;
        readonly IAzureVirtualMachineExtenedInfoService _azureVirtualMachineExtenedInfoService;

        public VirtualMachineService(ILogger<VirtualMachineService> logger,
            IConfiguration config,
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ISandboxService sandboxService,
            IVirtualMachineSizeService vmSizeService,
            IVirtualMachineLookupService vmLookupService,
            ICloudResourceCreateService sandboxResourceCreateService,
            ICloudResourceUpdateService sandboxResourceUpdateService,
            ICloudResourceDeleteService sandboxResourceDeleteService,
            ICloudResourceReadService sandboxResourceService,
            IProvisioningQueueService workQueue,
            IAzureVirtualMachineExtenedInfoService azureVirtualMachineExtenedInfoService)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _mapper = mapper;
            _userService = userService;
            _sandboxService = sandboxService;
            _vmSizeService = vmSizeService;
            _vmLookupService = vmLookupService;
            _sandboxResourceService = sandboxResourceService;
            _sandboxResourceCreateService = sandboxResourceCreateService;
            _sandboxResourceUpdateService = sandboxResourceUpdateService;
            _sandboxResourceDeleteService = sandboxResourceDeleteService;
            _workQueue = workQueue;
            _azureVirtualMachineExtenedInfoService = azureVirtualMachineExtenedInfoService;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, VirtualMachineCreateDto userInput)
        {
            CloudResource vmResourceEntry = null;

            try
            {
                ValidateVmPasswordOrThrow(userInput.Password);

                GenericNameValidation.ValidateName(userInput.Name);

                _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");

                var sandbox = await SandboxSingularQueries.GetSandboxByIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperation.Study_Crud_Sandbox, true);

                var virtualMachineName = AzureResourceNameUtil.VirtualMachine(sandbox.Study.Name, sandbox.Name, userInput.Name);

                await _sandboxResourceCreateService.ValidateThatNameDoesNotExistThrowIfInvalid(virtualMachineName);

                var tags = AzureResourceTagsFactory.SandboxResourceTags(_config, sandbox.Study, sandbox);

                var region = RegionStringConverter.Convert(sandbox.Region);

                userInput.DataDisks = await TranslateDiskSizes(sandbox.Region, userInput.DataDisks);

                var resourceGroup = await CloudResourceQueries.GetResourceGroupEntry(_db, sandboxId);

                //Make this dependent on bastion create operation to be completed, since bastion finishes last
                var dependsOn = await CloudResourceQueries.GetCreateOperationIdForBastion(_db, sandboxId);

                vmResourceEntry = await _sandboxResourceCreateService.CreateVmEntryAsync(sandboxId, resourceGroup, region.Name, tags, virtualMachineName, dependsOn, null);

                //Create vm settings and immeately attach to resource entry
                var vmSettingsString = await CreateVmSettingsString(sandbox.Region, vmResourceEntry.Id, sandbox.Study.Id, sandboxId, userInput);
                vmResourceEntry.ConfigString = vmSettingsString;
                await _sandboxResourceUpdateService.Update(vmResourceEntry.Id, vmResourceEntry);

                var queueParentItem = new ProvisioningQueueParentDto
                {                    
                    Description = $"Create VM for Sandbox: {sandboxId}"
                };

                queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = vmResourceEntry.Operations.FirstOrDefault().Id });

                await _workQueue.SendMessageAsync(queueParentItem);

                var dtoMappedFromResource = _mapper.Map<VmDto>(vmResourceEntry);

                return dtoMappedFromResource;
            }
            catch (Exception ex)
            {
                try
                {
                    //Delete resource if created
                    if (vmResourceEntry != null)
                    {
                        await _sandboxResourceDeleteService.HardDeletedAsync(vmResourceEntry.Id);
                    }
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, $"Failed to roll back VM creation for sandbox {sandboxId}");
                }

                throw new Exception($"Failed to create VM: {ex.Message}", ex);
            }
        }

        async Task<List<string>> TranslateDiskSizes(string region, List<string> dataDisksFromClient)
        {
            //Fix disks
            var disksFromDbForGivenRegion = await _db.RegionDiskSize.Where(rds => rds.RegionKey == region).Select(rds => rds.DiskSize).ToDictionaryAsync(ds => ds.Key, ds => ds);

            if (disksFromDbForGivenRegion.Count == 0)
            {
                throw new Exception($"No data disk items found in DB");
            }

            var result = new List<string>();

            foreach (var curDataDisk in dataDisksFromClient)
            {
                if (disksFromDbForGivenRegion.TryGetValue(curDataDisk, out DiskSize diskSize))
                {
                    result.Add(Convert.ToString(diskSize.Size));
                }
                else
                {
                    throw new Exception($"Unknown data disk size specification: {curDataDisk}");
                }
            }

            return result;
        }

        public Task<VmDto> UpdateAsync(int sandboxDto, VirtualMachineCreateDto newSandbox)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            var vmResource = await GetVmResourceEntry(id, UserOperation.Study_Crud_Sandbox);

            var deleteResourceOperation = await _sandboxResourceDeleteService.MarkAsDeletedWithDeleteOperationAsync(id);

            _logger.LogInformation($"Delete VM: Enqueing delete operation");

           var queueParentItem = QueueItemFactory.CreateParent(deleteResourceOperation);           

            await _workQueue.SendMessageAsync(queueParentItem);
        }

        public async Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default)
        {
            var sandbox = await _sandboxService.GetAsync(sandboxId, UserOperation.Study_Read);

            var virtualMachines = await CloudResourceQueries.GetSandboxVirtualMachinesList(_db, sandbox.Id);

            var virtualMachinesMapped = _mapper.Map<List<VmDto>>(virtualMachines);

            return virtualMachinesMapped;
        }

        public async Task<VmExtendedDto> GetExtendedInfo(int vmId, CancellationToken cancellationToken = default)
        {
            var vmResource = await GetVmResourceEntry(vmId, UserOperation.Study_Read);

            if (vmResource == null)
            {
                throw NotFoundException.CreateForCloudResource(vmId);
            }

            var dto = await _azureVirtualMachineExtenedInfoService.GetExtendedInfo(vmResource.ResourceGroupName, vmResource.ResourceName);

            var availableSizes = await _vmSizeService.AvailableSizes(vmResource.Region, cancellationToken);

            var availableSizesDict = availableSizes.ToDictionary(s => s.Key, s => s);

            if (!String.IsNullOrWhiteSpace(dto.SizeName) && availableSizesDict.TryGetValue(dto.SizeName, out VmSize curSize))
            {
                dto.Size = _mapper.Map<VmSizeDto>(curSize);
            }

            return dto;
        }

        public async Task<VmExternalLink> GetExternalLink(int vmId)
        {
            var vmResource = await GetVmResourceEntry(vmId, UserOperation.Study_Read);
            var vmExternalLink = new VmExternalLink
            {
                Id = vmId,
                LinkToExternalSystem = AzureResourceUtil.CreateResourceLink(_config, vmResource)
            };

            return vmExternalLink;
        }

        async Task<CloudResource> GetVmResourceEntry(int vmId, UserOperation operation)
        {
            _ = await StudySingularQueries.GetStudyByResourceIdCheckAccessOrThrow(_db, _userService, vmId, operation);
            var vmResource = await _sandboxResourceService.GetByIdAsync(vmId);

            return vmResource;
        }

        public void ValidateVmPasswordOrThrow(string password)
        {
            var errorString = "";
            //Atleast one upper case
            var upper = new Regex(@"(?=.*[A-Z])");
            //Atleast one number
            var number = new Regex(@".*[0-9].*");
            //Atleast one special character
            var special = new Regex(@"(?=.*[!@#$%^&*])");
            //Between 12-123 long
            var limit = new Regex(@"(?=.{12,123})");
            if (!upper.IsMatch(password))
            {
                errorString += "Missing one uppercase character. ";
            }
            if (!number.IsMatch(password))
            {
                errorString += "Missing one number. ";
            }
            if (!special.IsMatch(password))
            {
                errorString += "Missing one special character. ";
            }
            if (!limit.IsMatch(password))
            {
                errorString += "Outside the limit (12-123). ";

            }

            if (!String.IsNullOrWhiteSpace(errorString))
            {
                throw new Exception($"Password is missing following requirements: {errorString}");
            }

        }

        async Task<string> CreateVmSettingsString(string region, int vmId, int studyId, int sandboxId, VirtualMachineCreateDto userInput)
        {

            var vmSettings = _mapper.Map<VmSettingsDto>(userInput);

            var availableOs = await _vmLookupService.AvailableOperatingSystems(region);
            vmSettings.OperatingSystemCategory = AzureVmUtil.GetOsCategory(availableOs, vmSettings.OperatingSystem);

            vmSettings.Password = await StoreNewVmPasswordAsKeyVaultSecretAndReturnReference(studyId, sandboxId, vmSettings.Password);

            var diagStorageResource = await CloudResourceQueries.GetDiagStorageAccountEntry(_db, sandboxId);
            vmSettings.DiagnosticStorageAccountName = diagStorageResource.ResourceName;

            var networkResource = await CloudResourceQueries.GetNetworkEntry(_db, sandboxId);
            vmSettings.NetworkName = networkResource.ResourceName;

            var networkSetting = CloudResourceConfigStringSerializer.NetworkSettings(networkResource.ConfigString);
            vmSettings.SubnetName = networkSetting.SandboxSubnetName;

            vmSettings.Rules = AzureVmConstants.RulePresets.CreateInitialVmRules(vmId);
            return CloudResourceConfigStringSerializer.Serialize(vmSettings);
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
                throw new Exception($"Unable to store VM password in Key Vault. See inner exception for details.", ex);
            }
        }
    }
}
