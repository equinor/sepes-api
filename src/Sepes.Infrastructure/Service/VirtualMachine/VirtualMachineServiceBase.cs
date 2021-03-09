using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineServiceBase
    {

        protected readonly IConfiguration _config;
        protected readonly SepesDbContext _db;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;  

        readonly ISandboxModelService _sandboxModelService;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly IVirtualMachineOperatingSystemService _virtualMachineOperatingSystemService;
            
   

        public VirtualMachineServiceBase(
             IConfiguration config,
            SepesDbContext db
            , ILogger logger,           
            IMapper mapper,
            IUserService userService,         
            ISandboxModelService sandboxModelService,            
          
            ICloudResourceReadService cloudResourceReadService,           
            IVirtualMachineOperatingSystemService virtualMachineOperatingSystemService)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _mapper = mapper;
            _userService = userService;          
            _sandboxModelService = sandboxModelService;          
            _cloudResourceReadService = cloudResourceReadService; 
            _virtualMachineOperatingSystemService = virtualMachineOperatingSystemService;
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


       protected async Task<string> CreateVmSettingsString(string region, int vmId, int studyId, int sandboxId, VirtualMachineCreateDto userInput)
        {
            var vmSettings = _mapper.Map<VmSettingsDto>(userInput);

            var availableOs = await _virtualMachineOperatingSystemService.AvailableOperatingSystems(region);
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

        protected async Task<string> StoreNewVmPasswordAsKeyVaultSecretAndReturnReference(int studyId, int sandboxId, string password)
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

        protected async Task<CloudResource> GetVirtualMachineResourceEntry(int id, UserOperation operation, CancellationToken cancellation = default)
        {
            return await _cloudResourceReadService.GetByIdAsync(id, operation);
        } 
    }
}
