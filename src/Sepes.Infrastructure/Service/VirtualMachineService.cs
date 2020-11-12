using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        readonly ISandboxResourceService _sandboxResourceService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IProvisioningQueueService _workQueue;
        readonly IAzureVMService _azureVmService;
        readonly IAzureVmOsService _azureOsService;
        readonly IAzureCostManagementService _costService;

        public VirtualMachineService(ILogger<VirtualMachineService> logger,
            IConfiguration config,
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IStudyService studyService,
            ISandboxService sandboxService,
            ISandboxResourceService sandboxResourceService,
            ISandboxResourceOperationService sandboxResourceOperationService,
            IProvisioningQueueService workQueue,
            IAzureVMService azureVmService,
            IAzureCostManagementService costService,
            IAzureVmOsService azureOsService)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _mapper = mapper;
            _userService = userService;
            _studyService = studyService;
            _sandboxService = sandboxService;
            _sandboxResourceService = sandboxResourceService;
            _sandboxResourceOperationService = sandboxResourceOperationService;
            _workQueue = workQueue;
            _azureVmService = azureVmService;
            _azureOsService = azureOsService;
            _costService = costService;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto userInput)
        {
            _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");

            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);
            var study = await _studyService.GetStudyDtoByIdAsync(sandbox.StudyId, UserOperations.SandboxEdit);

            var virtualMachineName = AzureResourceNameUtil.VirtualMachine(study.Name, sandbox.Name, userInput.Name);

            await _sandboxResourceService.ValidateNameThrowIfInvalid(virtualMachineName);

            var tags = AzureResourceTagsFactory.CreateTags(_config, study, sandbox);

            var region = RegionStringConverter.Convert(sandbox.Region);          

            var resourceGroup = await SandboxResourceQueries.GetResourceGroupEntry(_db, sandboxId);

            //Make this dependent on bastion create operation to be completed, since bastion finishes last
            var dependsOn = await SandboxResourceQueries.GetCreateOperationIdForBastion(_db, sandboxId);

            var vmResourceEntry = await _sandboxResourceService.CreateVmEntryAsync(sandboxId, resourceGroup, region, tags, virtualMachineName, dependsOn, null);

            //Create vm settings and immeately attach to resource entry
            var vmSettingsString = await CreateVmSettingsString(sandbox.Region, vmResourceEntry.Id.Value, study.Id.Value, sandboxId, userInput);
            vmResourceEntry.ConfigString = vmSettingsString;
            await _sandboxResourceService.Update(vmResourceEntry.Id.Value, vmResourceEntry);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = sandboxId;
            queueParentItem.Description = $"Create VM for Sandbox: {sandboxId}";

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = vmResourceEntry.Operations.FirstOrDefault().Id.Value });

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
            var deletedResource = await _sandboxResourceService.MarkAsDeletedAndScheduleDeletion(id);

            var dtoMappedFromResource = _mapper.Map<VmDto>(deletedResource);

            return dtoMappedFromResource;
        }


        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        }

        public async Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            var virtualMachines = await SandboxResourceQueries.GetSandboxVirtualMachinesList(_db, sandbox.Id.Value);

            var virtualMachinesMapped = _mapper.Map<List<VmDto>>(virtualMachines);

            return virtualMachinesMapped;
        }

        public async Task<VmExtendedDto> GetExtendedInfo(int vmId)
        {
            var vmResourceQueryable = SandboxResourceQueries.GetSandboxResource(_db, vmId);
            var vmResource = await vmResourceQueryable.SingleOrDefaultAsync();

            if (vmResource == null)
            {
                throw NotFoundException.CreateForSandboxResource(vmId);
            }

            return await _azureVmService.GetExtendedInfo(vmResource.ResourceGroupName, vmResource.ResourceName);
        }

        async Task<SandboxResource> GetVmResourceEntry(int vmId, UserOperations operation)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyByResourceIdCheckAccessOrThrow(_db, _userService, vmId, operation);
            var vmResource = await _sandboxResourceService.GetByIdAsync(vmId);

            return vmResource;
        }

        public async Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput)
        {
            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            var vmPrice = await _costService.GetVmPrice(sandbox.Region, userInput.Size);

            return vmPrice;
        }

        void ThrowIfRuleExists(VmSettingsDto vmSettings, VmRuleDto ruleToCompare)
        {
            ThrowIfRuleExists(vmSettings.Rules, ruleToCompare);        
        }

        void ThrowIfRuleExists(List<VmRuleDto> rules, VmRuleDto ruleToCompare)
        {
            if (rules != null)
            {
                foreach (var curExistingRule in rules)
                {
                    if (AzureVmUtil.IsSameRule(ruleToCompare, curExistingRule))
                    {
                        throw new Exception($"Same rule allready exists");
                    }
                }
            }
        }

        public async Task<VmRuleDto> AddRule(int vmId, VmRuleDto input, CancellationToken cancellationToken = default)
        {
            await ValidateRuleThrowIfInvalid(vmId, input);

            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            ThrowIfRuleExists(vmSettings, input);

            input.Name = Guid.NewGuid().ToString();

            if (vmSettings.Rules == null)
            {
                vmSettings.Rules = new List<VmRuleDto>();
            }

            vmSettings.Rules.Add(input);

            vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

            await _db.SaveChangesAsync();

            await CreateUpdateOperationAndAddQueueItem(vm, "Add rule");

            return input;
        }



        public async Task<VmRuleDto> GetRuleById(int vmId, string ruleId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {
                foreach (var curExistingRule in vmSettings.Rules)
                {
                    if (curExistingRule.Name == ruleId)
                    {
                        return curExistingRule;
                    }
                }
            }

            throw new NotFoundException($"Rule with id {ruleId} does not exist");
        }

        public async Task<List<VmRuleDto>> SetRules(int vmId, List<VmRuleDto> updatedRuleSet, CancellationToken cancellationToken = default)
        {

            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            bool saveAfterwards = false;

            if (updatedRuleSet == null || updatedRuleSet != null && updatedRuleSet.Count == 0) //Easy, all rules should be deleted
            {
                vmSettings.Rules = null;
                saveAfterwards = true;
            }
            else
            {
                foreach(var curRule in updatedRuleSet)
                {
                    await ValidateRuleThrowIfInvalid(vmId, curRule);
                }

                var newRules = updatedRuleSet.Where(r => String.IsNullOrWhiteSpace(r.Name)).ToList();
                var existingRules = updatedRuleSet.Where(r => !String.IsNullOrWhiteSpace(r.Name)).ToList();

                foreach(var curNew in newRules)
                {
                    ThrowIfRuleExists(existingRules, curNew);
                }

                foreach (var curNewRule in updatedRuleSet)
                {
                    if (String.IsNullOrWhiteSpace(curNewRule.Name))
                    {
                        curNewRule.Name = Guid.NewGuid().ToString();
                    }
                }

                vmSettings.Rules = updatedRuleSet;
                saveAfterwards = true;
            }  
            
            if (saveAfterwards)
            {
                vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

                await _db.SaveChangesAsync();

                await CreateUpdateOperationAndAddQueueItem(vm, "Updated rules");
            }

            return updatedRuleSet != null? updatedRuleSet : new List<VmRuleDto>();
        }

        public async Task<VmRuleDto> UpdateRule(int vmId, VmRuleDto input, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {

                VmRuleDto ruleToRemove = null;

                var rulesDictionary = vmSettings.Rules.ToDictionary(r => r.Name, r => r);

                if (rulesDictionary.TryGetValue(input.Name, out ruleToRemove))
                {
                    vmSettings.Rules.Remove(ruleToRemove);

                    ThrowIfRuleExists(vmSettings, input);

                    vmSettings.Rules.Add(input);

                    vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

                    await _db.SaveChangesAsync();

                    await CreateUpdateOperationAndAddQueueItem(vm, "Update rule");

                    return input;
                }
            }

            throw new NotFoundException($"Rule with id {input.Name} does not exist");
        }

        public async Task<List<VmRuleDto>> GetRules(int vmId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            return vmSettings.Rules != null ? vmSettings.Rules : new List<VmRuleDto>();
        }

        public async Task<VmRuleDto> DeleteRule(int vmId, string ruleId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {

                VmRuleDto ruleToRemove = null;

                var rulesDictionary = vmSettings.Rules.ToDictionary(r => r.Name, r => r);

                if (rulesDictionary.TryGetValue(ruleId, out ruleToRemove))
                {
                    vmSettings.Rules.Remove(ruleToRemove);

                    vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

                    await _db.SaveChangesAsync();

                    await CreateUpdateOperationAndAddQueueItem(vm, "Delete rule");

                    return ruleToRemove;
                }
            }

            throw new NotFoundException($"Rule with id {ruleId} does not exist");
        }

        async Task ValidateRuleThrowIfInvalid(int vmId, VmRuleDto input)
        {
            if (true)
            {
                return;
            }

            throw new Exception($"Cannot apply rule to VM {vmId}");
        }

        async Task<SandboxResourceOperationDto> CreateUpdateOperationAndAddQueueItem(SandboxResource vm, string description)
        {
            var vmUpdateOperation = await _sandboxResourceOperationService.CreateUpdateOperationAsync(vm.Id);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = vm.SandboxId;
            queueParentItem.Description = $"Update VM state for Sandbox: {vm.SandboxId} ({description})";

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = vmUpdateOperation.Id.Value });

            await _workQueue.SendMessageAsync(queueParentItem);

            return vmUpdateOperation;
        }

        public async Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<VmSizeLookupDto> result = null;

            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            try
            {
                var availableVmSizesFromAzure = await _azureVmService.GetAvailableVmSizes(sandbox.Region, cancellationToken);

                var vmSizesWithCategory = availableVmSizesFromAzure.Select(sz =>
                new VmSizeLookupDto() { Key = sz.Name, DisplayValue = AzureVmUtil.GetDisplayTextSizeForDropdown(sz), Category = AzureVmUtil.GetSizeCategory(sz.Name) })
                    .ToList();

                result = vmSizesWithCategory.Where(s => s.Category != "unknowncategory" && (s.Category != "gpu" || (s.Category == "gpu" && s.Key == "Standard_NV8as_v4"))).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to get available VM sizes from azure for region {sandbox.Region}");
            }

            return result;
        }

        public async Task<List<VmDiskLookupDto>> AvailableDisks()
        {
            var result = new List<VmDiskLookupDto>();

            result.Add(new VmDiskLookupDto() { Key = "64", DisplayValue = "64 GB" });
            result.Add(new VmDiskLookupDto() { Key = "128", DisplayValue = "128 GB" });
            result.Add(new VmDiskLookupDto() { Key = "256", DisplayValue = "256 GB" });
            result.Add(new VmDiskLookupDto() { Key = "512", DisplayValue = "512 GB" });
            result.Add(new VmDiskLookupDto() { Key = "1024", DisplayValue = "1024 GB" });
            result.Add(new VmDiskLookupDto() { Key = "2048", DisplayValue = "2048 GB" });
            result.Add(new VmDiskLookupDto() { Key = "4096", DisplayValue = "4096 GB" });
            result.Add(new VmDiskLookupDto() { Key = "8192", DisplayValue = "8192 GB" });

            return result;
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<VmOsDto> result = null;

            try
            {
                var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

                result = await AvailableOperatingSystems(sandbox.Region, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var result = await  _azureOsService.GetAvailableOperatingSystemsAsync(region, cancellationToken); 

            var result = new List<VmOsDto>();

            ////Windows
            result.Add(new VmOsDto() { Key = "win2019datacenter", DisplayValue = "Windows Server 2019 Datacenter", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2019datacentercore", DisplayValue = "Windows Server 2019 Datacenter Core", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2016datacenter", DisplayValue = "Windows Server 2016 Datacenter", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2016datacentercore", DisplayValue = "Windows Server 2016 Datacenter Core", Category = "windows" });

            //Linux
            result.Add(new VmOsDto() { Key = "ubuntults", DisplayValue = "Ubuntu 1804 LTS", Category = "linux" });
            result.Add(new VmOsDto() { Key = "ubuntu16lts", DisplayValue = "Ubuntu 1604 LTS", Category = "linux" });
            result.Add(new VmOsDto() { Key = "rhel", DisplayValue = "RedHat 7 LVM", Category = "linux" });
            result.Add(new VmOsDto() { Key = "debian", DisplayValue = "Debian 10", Category = "linux" });
            result.Add(new VmOsDto() { Key = "centos", DisplayValue = "CentOS 7.5", Category = "linux" });

            return result;
        }

        async Task<string> CreateVmSettingsString(string region, int vmId, int studyId, int sandboxId, CreateVmUserInputDto userInput)
        {
            var vmSettings = _mapper.Map<VmSettingsDto>(userInput);

            var availableOs = await AvailableOperatingSystems(region);
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
