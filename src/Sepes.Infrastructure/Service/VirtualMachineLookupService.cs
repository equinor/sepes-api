using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.VirtualMachine;
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
    public class VirtualMachineLookupService : IVirtualMachineLookupService
    {
        readonly ILogger _logger;
        readonly ISandboxService _sandboxService;
        readonly IAzureVMService _azureVmService;
        readonly IAzureCostManagementService _costService;

        public VirtualMachineLookupService(ILogger<VirtualMachineService> logger,
            ISandboxService sandboxService,
            IProvisioningQueueService workQueue,
            IAzureVMService azureVmService,
            IAzureCostManagementService costService)
        {
            _logger = logger;

            _sandboxService = sandboxService;
            _azureVmService = azureVmService;
            _costService = costService;
        } 
        
        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        } 
        
        public async Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput)
        {
            var sandbox = await _sandboxService.GetSandboxAsync(sandboxId);

            var vmPrice = await _costService.GetVmPrice(sandbox.Region, userInput.Size);

            return vmPrice;
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
                var errorMessage = $"Unable to load available VM sizes for Sandbox {sandboxId}";

                if(sandbox != null)
                {
                    errorMessage += $", Region: {sandbox.Region}";
                }

                throw new Exception(errorMessage, ex);           
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

            //Windows
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


    }
}
