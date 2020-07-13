using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Sepes.Infrastructure.Util;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Sepes.Infrastructure.Service.Azure.Interface;

namespace Sepes.Infrastructure.Service
{
    public class AzureVMService : AzureServiceBase, IAzureVMService
    {       
        
       

        public AzureVMService(IConfiguration config, ILogger logger)
            :base (config, logger)
        {         
          
        }       

        public async Task<IVirtualMachine> Create(Region region, string resourceGroupName, string sandboxName, INetwork primaryNetwork, string subnetName, string userName, string password, string vmPerformanceProfile = "Cheap", string os = "Windows Server 2012")
        {
            var machineType = VirtualMachineSizeTypes.StandardD3V2;
            switch (vmPerformanceProfile)
            {
                case "Standard":
                    machineType = VirtualMachineSizeTypes.StandardD3V2;
                    break;
                case "Cheap":
                    machineType = VirtualMachineSizeTypes.StandardB1s;
                    break;
                default:
                    machineType = VirtualMachineSizeTypes.StandardB1s;
                    break;
            }

            var operatingSystem = KnownWindowsVirtualMachineImage.WindowsServer2012R2Datacenter;
            switch (os)
            {
                case "Windows Server 2012":
                    operatingSystem = KnownWindowsVirtualMachineImage.WindowsServer2012R2Datacenter;
                    break;
                case "Windows Server 2008":
                    operatingSystem = KnownWindowsVirtualMachineImage.WindowsServer2008R2_SP1;
                    break;
                default:
                    operatingSystem = KnownWindowsVirtualMachineImage.WindowsServer2012R2Datacenter;
                    break;
            }


            string virtualMachineName = AzureResourceNameUtil.VirtualMachine(sandboxName);
            var vm = await _azure.VirtualMachines.Define(virtualMachineName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithExistingPrimaryNetwork(primaryNetwork)
                .WithSubnet(subnetName)
                .WithPrimaryPrivateIPAddressDynamic()
                .WithoutPrimaryPublicIPAddress()
                .WithPopularWindowsImage(operatingSystem)
                .WithAdminUsername(userName)
                .WithAdminPassword(password)
                .WithSize(machineType)
                .CreateAsync();


            return vm;
        }

        public async Task ApplyVMStorageSettings(string resourceGroupName, string virtualMachineName)
        {
            // Not finished
            _azure.VirtualMachines.GetByResourceGroup(resourceGroupName, virtualMachineName).Update()
                .WithNewDataDisk(10);

            throw new NotImplementedException();
                
        }

        public async Task Delete(string resourceGroupName, string virtualMachineName)
        {
            await _azure.VirtualMachines.DeleteByResourceGroupAsync(resourceGroupName, virtualMachineName);
            return;
        }

    }
}
