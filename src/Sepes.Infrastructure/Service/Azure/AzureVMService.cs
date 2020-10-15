using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureVMService : AzureServiceBase, IAzureVMService
    {
        public AzureVMService(IConfiguration config, ILogger<AzureVMService> logger)
            : base(config, logger)
        {

        }

        public async Task<CloudResourceCRUDResult> Create(CloudResourceCRUDInput parameters)
        {
            _logger.LogInformation($"Creating VM: {parameters.SandboxName}! Resource Group: {parameters.ResourceGrupName}");

            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(parameters.CustomConfiguration);

            string diagStorageAccountName = vmSettings.DiagnosticStorageAccountName;

            string networkName = vmSettings.NetworkName;

            string subnetName = vmSettings.SubnetName;

            string username = vmSettings.Username;

            var passwordReference = vmSettings.Password;
            string password = await GetPasswordFromKeyVault(passwordReference);

            string performanceProfile = vmSettings.PerformanceProfile;
            string operatingSystem = vmSettings.OperatingSystem;
            string distro = vmSettings.Distro;

            var createdVm = await Create(parameters.Region, parameters.ResourceGrupName, parameters.Name, networkName, subnetName, username, password, performanceProfile, operatingSystem, distro, parameters.Tags, diagStorageAccountName);
            var result = CreateResult(createdVm);

            await DeletePasswordFromKeyVault(passwordReference);

            _logger.LogInformation($"Done creating Network Security Group for sandbox with Id: {parameters.SandboxId}! Id: {createdVm.Id}");
            return result;
        }

        async Task<string> GetPasswordFromKeyVault(string passwordId)
        {
            try
            {
                return await KeyVaultSecretUtil.GetKeyVaultSecretValue(_logger, _config, ConfigConstants.AZURE_VM_TEMP_PASSWORD_KEY_VAULT, passwordId);
            }
            catch (Exception ex)
            {

                throw new Exception($"VM Creation failed. Unable to get real VM password from Key Vault. See inner exception for details.", ex);
            }

        }

        async Task<string> DeletePasswordFromKeyVault(string passwordId)
        {
            try
            {
                return await KeyVaultSecretUtil.DeleteKeyVaultSecretValue(_logger, _config, ConfigConstants.AZURE_VM_TEMP_PASSWORD_KEY_VAULT, passwordId);
            }
            catch (Exception ex)
            {
                throw new Exception($"VM Creation failed. Unable to store VM password in Key Vault. See inner exception for details.", ex);
            }
        }

        public async Task<IVirtualMachine> Create(Region region, string resourceGroupName, string vmName, string primaryNetworkName, string subnetName, string userName, string password, string vmPerformanceProfile, string os, string distro, IDictionary<string, string> tags, string diagStorageAccountName)
        {
            IVirtualMachine vm;

            // Get diagnostic storage account reference for boot diagnostics
            var diagStorage = _azure.StorageAccounts.GetByResourceGroup(resourceGroupName, diagStorageAccountName);

            var network = await _azure.Networks.GetByResourceGroupAsync(resourceGroupName, primaryNetworkName);

            var vmCreatable = _azure.VirtualMachines.Define(vmName)
                                    .WithRegion(region)
                                    .WithExistingResourceGroup(resourceGroupName)
                                    .WithExistingPrimaryNetwork(network)
                                    .WithSubnet(subnetName)
                                    .WithPrimaryPrivateIPAddressDynamic()
                                    .WithoutPrimaryPublicIPAddress();

            VirtualMachineSizeTypes machineSize;

            switch (vmPerformanceProfile.ToLower())
            {
                case "general":
                    machineSize = VirtualMachineSizeTypes.StandardDS3V2;
                    break;
                case "cheap":
                    machineSize = VirtualMachineSizeTypes.StandardB1s;
                    break;
                case "high_compute":
                    machineSize = VirtualMachineSizeTypes.StandardH8;
                    break;
                case "high_memory":
                    machineSize = VirtualMachineSizeTypes.StandardM64s;
                    break;
                case "gpu":
                    machineSize = VirtualMachineSizeTypes.StandardND6s;
                    break;
                default:
                    machineSize = VirtualMachineSizeTypes.StandardB1s;
                    _logger.LogWarning($"Could not match vmPerformanceProfile argument: {vmPerformanceProfile}. Default will be chosen: StandardB1s");
                    break;
            }
            IWithCreate vmWithOS;
            if (os.ToLower().Equals("windows"))
            {
                vmWithOS = CreateWindowsVm(vmCreatable, distro, userName, password);
            }
            else if (os.ToLower().Equals("linux"))
            {
                vmWithOS = CreateLinuxVm(vmCreatable, distro, userName, password);
            }
            else
            {
                throw new ArgumentException($"Argument 'os' needs to be either 'windows' or 'linux'. Current value: {os}");
            }

            var vmWithSize = vmWithOS.WithSize(machineSize);
            vm = await vmWithSize
                .WithBootDiagnostics(diagStorage)
                .WithTags(tags)
                .CreateAsync();

            return vm;
        }

        private IWithWindowsCreateManagedOrUnmanaged CreateWindowsVm(IWithProximityPlacementGroup vmCreatable, string distro, string userName, string password)
        {
            IWithWindowsAdminUsernameManagedOrUnmanaged withOS;
            switch (distro.ToLower())
            {
                case "win2019datacenter":
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVMUtils.Windows.Server2019DataCenter.Publisher, AzureVMUtils.Windows.Server2019DataCenter.Offer, AzureVMUtils.Windows.Server2019DataCenter.Sku);
                    break;
                case "win2016datacenter":
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVMUtils.Windows.Server2016DataCenter.Publisher, AzureVMUtils.Windows.Server2016DataCenter.Offer, AzureVMUtils.Windows.Server2016DataCenter.Sku);
                    break;
                case "win2012r2datacenter":
                    withOS = vmCreatable.WithPopularWindowsImage(KnownWindowsVirtualMachineImage.WindowsServer2012R2Datacenter);
                    break;
                default:
                    _logger.LogInformation("Could not match distro argument. Default will be chosen: Windows Server 2019");
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVMUtils.Windows.Server2019DataCenter.Publisher, AzureVMUtils.Windows.Server2019DataCenter.Offer, AzureVMUtils.Windows.Server2019DataCenter.Sku);
                    break;
            }
            var vm = withOS
                .WithAdminUsername(userName)
                .WithAdminPassword(password);
            return vm;
        }

        private IWithLinuxCreateManagedOrUnmanaged CreateLinuxVm(IWithProximityPlacementGroup vmCreatable, string distro, string userName, string password)
        {
            IWithLinuxRootUsernameManagedOrUnmanaged withOS;
            switch (distro.ToLower())
            {
                case "ubuntults":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVMUtils.Linux.UbuntuServer1804LTS.Publisher, AzureVMUtils.Linux.UbuntuServer1804LTS.Offer, AzureVMUtils.Linux.UbuntuServer1804LTS.Sku);
                    break;
                case "ubuntu16lts":
                    withOS = vmCreatable.WithPopularLinuxImage(KnownLinuxVirtualMachineImage.UbuntuServer16_04_Lts);
                    break;
                case "rhel":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVMUtils.Linux.RedHat7LVM.Publisher, AzureVMUtils.Linux.RedHat7LVM.Offer, AzureVMUtils.Linux.RedHat7LVM.Sku);
                    break;
                case "debian":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVMUtils.Linux.Debian10.Publisher, AzureVMUtils.Linux.Debian10.Offer, AzureVMUtils.Linux.Debian10.Sku);
                    break;
                case "centos":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVMUtils.Linux.CentOS75.Publisher, AzureVMUtils.Linux.CentOS75.Offer, AzureVMUtils.Linux.CentOS75.Sku);
                    break;
                default:
                    _logger.LogInformation("Could not match distro argument. Default will be chosen: Ubuntu 18.04-LTS");
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVMUtils.Linux.UbuntuServer1804LTS.Publisher, AzureVMUtils.Linux.UbuntuServer1804LTS.Offer, AzureVMUtils.Linux.UbuntuServer1804LTS.Sku);
                    break;
            }
            var vm = withOS
                .WithRootUsername(userName)
                .WithRootPassword(password);

            return vm;
        }



        public async Task ApplyVMStorageSettings(string resourceGroupName, string virtualMachineName, int sizeInGB, string type)
        {
            var vm = await GetResourceAsync(resourceGroupName, virtualMachineName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, vm.Tags);

            // Not finished
            vm.Update()
                .WithNewDataDisk(sizeInGB);

            throw new NotImplementedException();

        }

        public async Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters)
        {
            await Delete(parameters.ResourceGrupName, parameters.Name);

            var provisioningState = await GetProvisioningState(parameters.ResourceGrupName, parameters.Name);
            var crudResult = CloudResourceCRUDUtil.CreateResultFromProvisioningState(provisioningState);
            return crudResult;

        }



        public async Task Delete(string resourceGroupName, string virtualMachineName)
        {
            var vm = await GetResourceAsync(resourceGroupName, virtualMachineName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, vm.Tags);

            await _azure.VirtualMachines.DeleteByResourceGroupAsync(resourceGroupName, virtualMachineName);
            return;
        }

        public async Task<IVirtualMachine> GetResourceAsync(string resourceGroupName, string resourceName)
        {
            var resource = await _azure.VirtualMachines.GetByResourceGroupAsync(resourceGroupName, resourceName);
            return resource;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource.ProvisioningState;
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName);
            return AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(resource.Tags);
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var resource = await GetResourceAsync(resourceGroupName, resourceName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

            _ = await resource.Update().WithoutTag(tag.Key).ApplyAsync();
            _ = await resource.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }

        CloudResourceCRUDResult CreateResult(IVirtualMachine vm)
        {
            var crudResult = CloudResourceCRUDUtil.CreateResultFromIResource(vm);
            crudResult.CurrentProvisioningState = vm.Inner.ProvisioningState.ToString();
            return crudResult;
        }
    }
}
