using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Sepes.Infrastructure.Util;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition;
using System.Collections.Generic;
using Microsoft.Azure.Management.Storage.Fluent;

namespace Sepes.Infrastructure.Service
{
    public class AzureVMService : AzureServiceBase, IAzureVMService
    {
        public AzureVMService(IConfiguration config, ILogger logger)
            :base (config, logger)
        {         
          
        }       

        public async Task<IVirtualMachine> Create(Region region, string resourceGroupName, string sandboxName, INetwork primaryNetwork, string subnetName, string userName, string password, string vmPerformanceProfile, string os, string distro, IDictionary<string, string> tags, string diagStorageAccountName)
        {
            IVirtualMachine vm;
            string virtualMachineName = AzureResourceNameUtil.VirtualMachine(sandboxName);
            // Get diagnostic storage account reference for boot diagnostics
            IStorageAccount diagStorage = _azure.StorageAccounts.GetByResourceGroup(resourceGroupName, diagStorageAccountName);


            var vmCreatable = _azure.VirtualMachines.Define(virtualMachineName)
                                    .WithRegion(region)
                                    .WithExistingResourceGroup(resourceGroupName)
                                    .WithExistingPrimaryNetwork(primaryNetwork)
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
                    _logger.LogInformation($"Could not match vmPerformanceProfile argument: {vmPerformanceProfile}. Default will be chosen: StandardB1s");
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
            // Not finished
            _azure.VirtualMachines.GetByResourceGroup(resourceGroupName, virtualMachineName).Update()
                .WithNewDataDisk(sizeInGB);

            throw new NotImplementedException();
                
        }

        public async Task Delete(string resourceGroupName, string virtualMachineName)
        {
            await _azure.VirtualMachines.DeleteByResourceGroupAsync(resourceGroupName, virtualMachineName);
            return;
        }

        public async Task<bool> Exists(string resourceGroupName, string virtualMachineName)
        {
            var vm = await _azure.VirtualMachines.GetByResourceGroupAsync(resourceGroupName, virtualMachineName);

            if (vm == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(vm.Id);
        }
    }
}
