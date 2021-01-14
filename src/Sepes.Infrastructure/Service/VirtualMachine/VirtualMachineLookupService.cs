using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Sepes.Infrastructure.Constants;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineLookupService : IVirtualMachineLookupService
    {
        readonly ILogger _logger;
        readonly ISandboxService _sandboxService;
        readonly IAzureCostManagementService _costService;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        public VirtualMachineLookupService(
            ILogger<VirtualMachineService> logger,
            SepesDbContext db,
            IMapper mapper,
            ISandboxService sandboxService,
            IAzureCostManagementService costService)
        {
            _db = db;
            _logger = logger;
            _sandboxService = sandboxService;
            _costService = costService;
            _mapper = mapper;
        }

        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        }

        public async Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput, CancellationToken cancellationToken = default)
        {
            var sandbox = await _sandboxService.GetAsync(sandboxId, Constants.UserOperation.Study_Crud_Sandbox);

            var vmPrice = await _costService.GetVmPrice(sandbox.Region, userInput.Size, cancellationToken);

            return vmPrice;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var vmDisks = await _db.DiskSizes.Include(x => x).ToListAsync();
            var sortedByPrice = vmDisks.OrderBy(x => x.Size);
            /*
            var result = new List<VmDiskLookupDto>
            {
                new VmDiskLookupDto() { Key = "64", DisplayValue = "64 GB" },
                new VmDiskLookupDto() { Key = "128", DisplayValue = "128 GB" },
                new VmDiskLookupDto() { Key = "256", DisplayValue = "256 GB" },
                new VmDiskLookupDto() { Key = "512", DisplayValue = "512 GB" },
                new VmDiskLookupDto() { Key = "1024", DisplayValue = "1024 GB" },
                new VmDiskLookupDto() { Key = "2048", DisplayValue = "2048 GB" },
                new VmDiskLookupDto() { Key = "4096", DisplayValue = "4096 GB" },
                new VmDiskLookupDto() { Key = "8192", DisplayValue = "8192 GB" }
            };
            */

            return _mapper.Map<List<VmDiskLookupDto>>(sortedByPrice);
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            List<VmOsDto> result = null;

            try
            {
                var sandbox = await _sandboxService.GetAsync(sandboxId, Constants.UserOperation.Study_Crud_Sandbox);

                result = await AvailableOperatingSystems(sandbox.Region, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

            return result;
        }

        public Boolean CheckIfUsernameIsValidOrThrow(string userName)
        {
            var errorString = "";
            var listOfInvalidNames = "";
            if (userName.EndsWith("."))
            {
                errorString += "Name can not end with a period(.)";
            }
            foreach (string invalidName in AzureVmInvalidUsernames.invalidUsernames)
            {
                if (userName.Equals(invalidName))
                {
                    errorString += $"The name: '{userName}' is not valid.";
                    foreach (string name in AzureVmInvalidUsernames.invalidUsernames)
                    {
                        listOfInvalidNames += name;
                        if (name != AzureVmInvalidUsernames.invalidUsernames.Last())
                        {
                            listOfInvalidNames += ", ";
                        }
                    }
                    errorString += $" The following names are not allowed: {listOfInvalidNames}";
                    break;
                }
            }

            if (!String.IsNullOrWhiteSpace(errorString))
            {
                throw new Exception($"{errorString}.");
            }
            return true;
        }
    }
}
