using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineLookupService : IVirtualMachineLookupService
    {
        readonly ILogger _logger;
        readonly ISandboxService _sandboxService;
        readonly SepesDbContext _db;

        public VirtualMachineLookupService(
            ILogger<VirtualMachineService> logger,
            SepesDbContext db,        
            ISandboxService sandboxService)
        {
            _db = db;
            _logger = logger;
            _sandboxService = sandboxService; 
        }

        public string CalculateName(string studyName, string sandboxName, string userPrefix)
        {
            return AzureResourceNameUtil.VirtualMachine(studyName, sandboxName, userPrefix);
        }      

        public async Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default)
        {
           return await _db.DiskSizes.OrderBy(x => x.Size).Select(ds=> new VmDiskLookupDto(){ Key = ds.Key, DisplayValue = ds.DisplayText }).ToListAsync(); 
        }

        public async Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default)
        {
            List<VmOsDto> result = null;

            try
            {
                var sandbox = await _sandboxService.GetAsync(sandboxId, UserOperation.Study_Crud_Sandbox);

                result = await AvailableOperatingSystems(sandbox.Region, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Unable to get available OS from azure for sandbox {sandboxId}");
            }

            return result;
        }


        public async Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default)

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

        public VmUsernameValidateDto CheckIfUsernameIsValidOrThrow(VmUsernameDto input)
        {
            StringBuilder errorString = new StringBuilder("");
            StringBuilder listOfInvalidNames = new StringBuilder("");
            VmUsernameValidateDto usernameValidation = new VmUsernameValidateDto { errorMessage = "", isValid = true };
            if (input.Username.EndsWith("."))
            {
                usernameValidation.isValid = false;
                errorString.Append("Name can not end with a period(.)");
            }
            var invalidUsernames = AzureVmInvalidUsernames.invalidUsernamesWindows;
            if (input.OperativeSystemType == AzureVmConstants.LINUX)
            {
                invalidUsernames = AzureVmInvalidUsernames.invalidUsernamesLinux;
            }
            foreach (string invalidName in invalidUsernames)
            {
                if (input.Username.Equals(invalidName))
                {
                    usernameValidation.isValid = false;
                    errorString.Append($"The name: '{input.Username}' is not valid.");
                    foreach (string name in invalidUsernames)
                    {
                        listOfInvalidNames.Append(name);
                        if (name != invalidUsernames.Last())
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
