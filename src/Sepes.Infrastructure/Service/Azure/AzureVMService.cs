using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition;
using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureVMService : AzureServiceBase, IAzureVMService
    {
        public AzureVMService(IConfiguration config, ILogger<AzureVMService> logger)
            : base(config, logger)
        {

        }

        public async Task<CloudResourceCRUDResult> EnsureCreatedAndConfigured(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInformation($"Creating VM: {parameters.Name} in resource Group: {parameters.ResourceGrupName}");

            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(parameters.CustomConfiguration);

            var passwordReference = vmSettings.Password;
            string password = await GetPasswordFromKeyVault(passwordReference);

            string vmSize = vmSettings.Size;


            var createdVm = await Create(parameters.Region,
                parameters.ResourceGrupName,
                parameters.Name,
                vmSettings.NetworkName, vmSettings.SubnetName,
                vmSettings.Username, password,
                vmSize, vmSettings.OperatingSystem, vmSettings.OperatingSystemCategory, parameters.Tags,
                vmSettings.DiagnosticStorageAccountName, cancellationToken);

            if (vmSettings.DataDisks != null && vmSettings.DataDisks.Count > 0)
            {
                foreach (var curDisk in vmSettings.DataDisks)
                {
                    var sizeAsInt = Convert.ToInt32(curDisk);

                    if (sizeAsInt == 0)
                    {
                        throw new Exception($"Illegal data disk size: {curDisk}");
                    }

                    await ApplyVmDataDisks(parameters.ResourceGrupName, parameters.Name, sizeAsInt);
                }
            }

            var result = CreateCRUDResult(createdVm);

            await DeletePasswordFromKeyVault(passwordReference);

            _logger.LogInformation($"Done creating Network Security Group for sandbox with Id: {parameters.SandboxId}! Id: {createdVm.Id}");
            return result;
        }
        public async Task<CloudResourceCRUDResult> GetSharedVariables(CloudResourceCRUDInput parameters)
        {
            var vm = await GetAsync(parameters.ResourceGrupName, parameters.Name);
            var result = CreateCRUDResult(vm);
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

                throw new Exception($"VM Creation failed. Unable to get VM password from Key Vault. See inner exception for details.", ex);
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
                throw new Exception($"VM Creation failed. Unable to delete VM password from Key Vault after use. See inner exception for details.", ex);
            }
        }

        public async Task<IVirtualMachine> Create(Region region, string resourceGroupName, string vmName, string primaryNetworkName, string subnetName, string userName, string password, string vmSize, string osName, string osCategory, IDictionary<string, string> tags, string diagStorageAccountName, CancellationToken cancellationToken = default(CancellationToken))
        {
            IVirtualMachine vm;

            // Get diagnostic storage account reference for boot diagnostics
            var diagStorage = _azure.StorageAccounts.GetByResourceGroup(resourceGroupName, diagStorageAccountName);

            AzureResourceUtil.ThrowIfResourceIsNull(diagStorage, AzureResourceType.StorageAccount, diagStorageAccountName, "Create VM failed");

            var network = await _azure.Networks.GetByResourceGroupAsync(resourceGroupName, primaryNetworkName);

            AzureResourceUtil.ThrowIfResourceIsNull(network, AzureResourceType.VirtualNetwork, primaryNetworkName, "Create VM failed");

            var vmCreatable = _azure.VirtualMachines.Define(vmName)
                                    .WithRegion(region)
                                    .WithExistingResourceGroup(resourceGroupName)
                                    .WithExistingPrimaryNetwork(network)
                                    .WithSubnet(subnetName)
                                    .WithPrimaryPrivateIPAddressDynamic()
                                    .WithoutPrimaryPublicIPAddress();


            IWithCreate vmWithOS;

            if (osCategory.ToLower().Equals("windows"))
            {
                vmWithOS = CreateWindowsVm(vmCreatable, osName, userName, password);
            }
            else if (osCategory.ToLower().Equals("linux"))
            {
                vmWithOS = CreateLinuxVm(vmCreatable, osName, userName, password);
            }
            else
            {
                throw new ArgumentException($"Argument 'osCategory' needs to be either 'windows' or 'linux'. Current value: {osCategory}");
            }

            var vmWithSize = vmWithOS.WithSize(vmSize);

            vm = await vmWithSize
                .WithBootDiagnostics(diagStorage)
                .WithTags(tags)
                .CreateAsync(cancellationToken);

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



        public async Task ApplyVmDataDisks(string resourceGroupName, string virtualMachineName, int sizeInGB)
        {
            var vm = await GetAsync(resourceGroupName, virtualMachineName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, vm.Tags);

            var updatedVm = await vm.Update()
                 .WithNewDataDisk(sizeInGB).ApplyAsync();

        }

        public async Task<CloudResourceCRUDResult> Delete(CloudResourceCRUDInput parameters)
        {
            string provisioningState = null;

            try
            {
                await Delete(parameters.ResourceGrupName, parameters.Name);

                //Also remember to delete osdisk
                provisioningState = await GetProvisioningState(parameters.ResourceGrupName, parameters.Name);

            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Virtual Machine {parameters.Name} appears to be deleted allready");
                provisioningState = CloudResourceProvisioningStates.NOTFOUND;
                //Probably allready deleted

            }

            return CloudResourceCRUDUtil.CreateResultFromProvisioningState(provisioningState);

        }


        public async Task Delete(string resourceGroupName, string virtualMachineName)
        {
            var vm = await GetAsync(resourceGroupName, virtualMachineName);

            if (vm == null)
            {
                _logger.LogWarning($"Virtual Machine {virtualMachineName} not found in RG {resourceGroupName}");
                return;
            }

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, vm.Tags);

            await _azure.VirtualMachines.DeleteByResourceGroupAsync(resourceGroupName, virtualMachineName);

            //Delete all the disks
            await DeleteDiskById(vm.OSDiskId);

            foreach (var curNic in vm.NetworkInterfaceIds)
            {
                await DeleteNic(curNic);
            }

            foreach (var curDiskKvp in vm.DataDisks)
            {
                await DeleteDiskById(curDiskKvp.Value.Id);
            }
        }

        public async Task DeleteNic(string id)
        {
            await _azure.NetworkInterfaces.DeleteByIdAsync(id);
        }

        public async Task DeleteDiskById(string id)
        {
            await _azure.Disks.DeleteByIdAsync(id);
        }

        public async Task<IVirtualMachine> GetAsync(string resourceGroupName, string resourceName)
        {
            var resource = await _azure.VirtualMachines.GetByResourceGroupAsync(resourceGroupName, resourceName);
            return resource;
        }

        public async Task<string> GetProvisioningState(string resourceGroupName, string resourceName)
        {
            var resource = await GetAsync(resourceGroupName, resourceName);

            if (resource == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceName, resourceGroupName);
            }

            return resource.ProvisioningState;
        }

        public async Task<IDictionary<string, string>> GetTagsAsync(string resourceGroupName, string resourceName)
        {
            var resource = await GetAsync(resourceGroupName, resourceName);
            return AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(resource.Tags);
        }

        public async Task UpdateTagAsync(string resourceGroupName, string resourceName, KeyValuePair<string, string> tag)
        {
            var resource = await GetAsync(resourceGroupName, resourceName);

            //Ensure resource is is managed by this instance
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceGroupName, resource.Tags);

            _ = await resource.Update().WithoutTag(tag.Key).ApplyAsync();
            _ = await resource.Update().WithTag(tag.Key, tag.Value).ApplyAsync();
        }

        CloudResourceCRUDResult CreateCRUDResult(IVirtualMachine vm)
        {
            var crudResult = CloudResourceCRUDUtil.CreateResultFromIResource(vm);
            crudResult.CurrentProvisioningState = vm.Inner.ProvisioningState.ToString();
            return crudResult;
        }

        public async Task<IEnumerable<VirtualMachineSize>> GetAvailableVmSizes(string region = null, CancellationToken cancellationToken = default)
        {
            using (var client = new Microsoft.Azure.Management.Compute.ComputeManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var sizes = await client.VirtualMachineSizes.ListWithHttpMessagesAsync(region, cancellationToken: cancellationToken);
                var sizesResponseText = await sizes.Response.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<AzureVirtualMachineSizeResponse>(sizesResponseText);
                return deserialized.Value;
            }
        }

        public async Task<VmExtendedDto> GetExtendedInfo(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default)
        {
            var vm = await GetAsync(resourceGroupName, resourceName);

            if (vm == null)
            {
                throw NotFoundException.CreateForAzureResource(resourceGroupName, resourceName);
            }

            var result = new VmExtendedDto();

            result.PowerState = AzureVmUtil.GetPowerState(vm);

            result.OsType = AzureVmUtil.GetOsType(vm);

            result.SizeName = vm.Size.ToString();
          
            var availableSizes = await GetAvailableVmSizes(vm.RegionName, cancellationToken);

            var availableSizesDict = availableSizes.ToDictionary(s => s.Name, s => s);

            VirtualMachineSize curSize = null;

            if (availableSizesDict.TryGetValue(result.SizeName, out curSize))
            {
                result.Size = new VmSizeDto() { Name = result.SizeName, MemoryInMB = curSize.MemoryInMB.Value, MaxDataDiskCount = curSize.MaxDataDiskCount.Value, NumberOfCores = curSize.NumberOfCores.Value, OsDiskSizeInMB = curSize.OsDiskSizeInMB.Value, ResourceDiskSizeInMB = curSize.ResourceDiskSizeInMB.Value };
            }            

            result.NICs.Add(await CreateNicDto(vm.PrimaryNetworkInterfaceId));

            foreach (var curNic in vm.NetworkInterfaceIds)
            {
                result.NICs.Add(await CreateNicDto(curNic));
            }

            result.Disks.Add(await CreateDiskDto(vm.OSDiskId, true));

            foreach (var curDiskKvp in vm.DataDisks.Values)
            {
                result.Disks.Add(CreateDiskDto(curDiskKvp, false));
            }

            return result;
        }



        async Task<VmNicDto> CreateNicDto(string nicId)
        {
            var nic = await _azure.NetworkInterfaces.GetByIdAsync(nicId);

            if (nic == null)
            {
                throw NotFoundException.CreateForAzureResourceById(nicId);
            }

            var result = new VmNicDto() { Name = nic.Name };
            return result;
        }

        async Task<VmDiskDto> CreateDiskDto(string diskId, bool isOs)
        {
            var disk = await _azure.Disks.GetByIdAsync(diskId);

            if (disk == null)
            {
                throw NotFoundException.CreateForAzureResourceById(diskId);
            }

            var result = new VmDiskDto() { Name = disk.Name, CapacityGb = disk.SizeInGB, Category = isOs ? "os" : "data" };

            return result;
        }

        VmDiskDto CreateDiskDto(IVirtualMachineDataDisk disk, bool isOs)
        {
            var result = new VmDiskDto() { Name = disk.Name, CapacityGb = disk.Size, Category = isOs ? "os" : "data" };

            return result;
        }


    }
}
