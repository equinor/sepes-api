using Microsoft.Extensions.Logging;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineOperatingSystemService : IVirtualMachineOperatingSystemService
    {
        readonly ILogger _logger;    
        //readonly ISandboxModelService _sandboxModelService;

        public VirtualMachineOperatingSystemService(
            ILogger<VirtualMachineOperatingSystemService> logger     
            //ISandboxModelService sandboxModelService
            )
        {     
            _logger = logger;     
            //_sandboxModelService = sandboxModelService;            
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            List<VmOsDto> result = null;

            try
            {
                //Don't need this now, but will in the near future
                //var sandboxRegion = await _sandboxModelService.GetRegionByIdAsync(sandboxId, UserOperation.Study_Crud_Sandbox);

                result = await AvailableOperatingSystems(region: null, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }


        public async Task<List<VmOsDto>> AvailableOperatingSystems(string region = null, CancellationToken cancellationToken = default)

        {
            //var result = await  _azureOsService.GetAvailableOperatingSystemsAsync(region, cancellationToken); 

            var result = new List<VmOsDto>
            {

                //Windows
                new VmOsDto() { Key = "win2019datacenter", DisplayValue = "Windows Server 2019 Datacenter", Category = "windows" },
                new VmOsDto() { Key = "win2019datacentercore", DisplayValue = "Windows Server 2019 Datacenter Core", Category = "windows" },
                new VmOsDto() { Key = "win2016datacenter", DisplayValue = "Windows Server 2016 Datacenter", Category = "windows" },
                new VmOsDto() { Key = "win2016datacentercore", DisplayValue = "Windows Server 2016 Datacenter Core", Category = "windows" },

                //Linux
                new VmOsDto() { Key = "ubuntults", DisplayValue = "Ubuntu 1804 LTS", Category = "linux" },
                new VmOsDto() { Key = "ubuntu16lts", DisplayValue = "Ubuntu 1604 LTS", Category = "linux" },
                new VmOsDto() { Key = "rhel", DisplayValue = "RedHat 7 LVM", Category = "linux" },
                new VmOsDto() { Key = "debian", DisplayValue = "Debian 10", Category = "linux" },
                new VmOsDto() { Key = "centos", DisplayValue = "CentOS 7.5", Category = "linux" }
            };

            return await Task.FromResult(result);
        }      
    }
}
