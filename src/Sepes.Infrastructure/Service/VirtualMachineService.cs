using AutoMapper;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Exceptions;
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
        readonly IProvisioningQueueService _workQueue;
        readonly IAzureVMService _azureVmService;
        readonly IAzureCostManagementService _costService;

        public VirtualMachineService(ILogger<VirtualMachineService> logger,
            IConfiguration config,
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IStudyService studyService,
            ISandboxService sandboxService,
            ISandboxResourceService sandboxResourceService,
            IProvisioningQueueService workQueue,
            IAzureVMService azureVmService,
            IAzureCostManagementService costService)
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
            _azureVmService = azureVmService;
            _costService = costService;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto userInput)
        {
         

            _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");

            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);
            var study = await _studyService.GetStudyByIdAsync(sandbox.StudyId);
            await _costService.GetVmPrice(sandbox.Region, userInput.Size);
            var virtualMachineName = AzureResourceNameUtil.VirtualMachine(study.Name, sandbox.Name, userInput.Name);

            await _sandboxResourceService.ValidateNameThrowIfInvalid(virtualMachineName);

            var tags = AzureResourceTagsFactory.CreateTags(_config, study, sandbox);

            var region = RegionStringConverter.Convert(sandbox.Region);

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

            if(vmResource == null)
            {
                throw NotFoundException.CreateForSandboxResource(vmId);
            }

            return await _azureVmService.GetExtendedInfo(vmResource.ResourceGroupName, vmResource.ResourceName);  
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

        public async Task<List<VmOsDto>> AvailableOperatingSystems()
        {
            var result = new List<VmOsDto>();

            //Windows
            result.Add(new VmOsDto() { Key = "win2019datacenter", DisplayValue = "Windows Server 2019 Datacenter", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2016datacenter", DisplayValue = "Windows Server 2016 Datacenter", Category = "windows" });
            result.Add(new VmOsDto() { Key = "win2012r2datacenter", DisplayValue = "Windows Server 2012 Datacenter R2", Category = "windows" });

            //Linux
            result.Add(new VmOsDto() { Key = "ubuntults", DisplayValue = "Ubuntu 1804 LTS", Category = "linux" });
            result.Add(new VmOsDto() { Key = "ubuntu16lts", DisplayValue = "Ubuntu 1604 LTS", Category = "linux" });
            result.Add(new VmOsDto() { Key = "rhel", DisplayValue = "RedHat 7 LVM", Category = "linux" });
            result.Add(new VmOsDto() { Key = "debian", DisplayValue = "Debian 10", Category = "linux" });
            result.Add(new VmOsDto() { Key = "centos", DisplayValue = "CentOS 7.5", Category = "linux" });

            return result;
        }

        async Task<string> CreateVmSettingsString(int studyId, int sandboxId, CreateVmUserInputDto userInput)
        {
            var vmSettings = _mapper.Map<VmSettingsDto>(userInput);

            var availableOs = await AvailableOperatingSystems();
            vmSettings.OperatingSystemCategory = AzureVmUtil.GetOsCategory(availableOs, vmSettings.OperatingSystem);

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
