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
using System.Text;

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

        public async Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default)

        {
            var vmDisks = await _db.DiskSizes.ToListAsync();
            var sortedByPrice = vmDisks.OrderBy(x => x.Size);
           

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

        public VmUsernameValidateDto CheckIfUsernameIsValidOrThrow(string userName)
        {
            StringBuilder errorString = new StringBuilder("");
            StringBuilder listOfInvalidNames = new StringBuilder("");
            VmUsernameValidateDto usernameValidation = new VmUsernameValidateDto { errorMessage = "", isValid = true };
            if (userName.EndsWith("."))
            {
                usernameValidation.isValid = false;
                errorString.Append("Name can not end with a period(.)");
            }
            foreach (string invalidName in AzureVmInvalidUsernames.invalidUsernames)
            {
                if (userName.Equals(invalidName))
                {
                    usernameValidation.isValid = false;
                    errorString.Append($"The name: '{userName}' is not valid.");
                    foreach (string name in AzureVmInvalidUsernames.invalidUsernames)
                    {
                        listOfInvalidNames.Append(name);
                        if (name != AzureVmInvalidUsernames.invalidUsernames.Last())
                        {
                            listOfInvalidNames.Append(", ");
                        }
                    }
                    errorString.Append($" The following names are not allowed: {listOfInvalidNames}");
                    break;
                }
            }
            usernameValidation.errorMessage = errorString.ToString();
            return usernameValidation;
        }
    }
}
