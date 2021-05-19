using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Azure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Util;
using Sepes.Common.Exceptions;

namespace Sepes.Azure.Service
{
    public class AzureVirtualMachineExtendedInfoService : AzureVirtualMachineServiceBase, IAzureVirtualMachineExtendedInfoService
    { 
        public AzureVirtualMachineExtendedInfoService(IConfiguration config, ILogger<AzureVirtualMachineExtendedInfoService> logger)
            : base(config, logger)
        {
         
        }  

        public async Task<VmExtendedDto> GetExtendedInfo(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default)
        {
            var vm = await GetInternalAsync(resourceGroupName, resourceName);

            var result = new VmExtendedDto
            {
                PowerState = AzureVmUtil.GetPowerState(vm),
                OsType = AzureVmUtil.GetOsType(vm)
            };

            if (vm == null)
            {
                return result;
            }

            result.SizeName = vm.Size.ToString();

            await DecorateWithNetworkProperties(vm, result, cancellationToken);

            result.Disks.Add(await CreateDiskDto(vm.OSDiskId, true, cancellationToken));

            foreach (var curDiskKvp in vm.DataDisks.Values)
            {
                result.Disks.Add(CreateDiskDto(curDiskKvp, false));
            }

            return result;
        }


        async Task DecorateWithNetworkProperties(IVirtualMachine vm, VmExtendedDto vmDto, CancellationToken cancellationToken)
        {
            var primaryNic = await _azure.NetworkInterfaces.GetByIdAsync(vm.PrimaryNetworkInterfaceId, cancellationToken);

            vmDto.PrivateIp = primaryNic.PrimaryPrivateIP;

            try
            {
                if (primaryNic.PrimaryIPConfiguration != null)
                {
                    var pip = await _azure.PublicIPAddresses.GetByResourceGroupAsync(vm.ResourceGroupName, primaryNic.PrimaryIPConfiguration.Name, cancellationToken);

                    if (pip != null)
                    {
                        vmDto.PublicIp = pip.IPAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Unable to fetch public IP settings for VM {vm.Name}");
            }

            vmDto.NICs.Add(CreateNicDto(primaryNic));

            foreach (var curNic in vm.NetworkInterfaceIds)
            {
                vmDto.NICs.Add(await CreateNicDto(curNic, cancellationToken));
            }
        }

        VmNicDto CreateNicDto(INetworkInterface nic)
        {
            var result = new VmNicDto() { Name = nic.Name };
            return result;
        }

        async Task<VmNicDto> CreateNicDto(string nicId, CancellationToken cancellationToken)
        {
            var nic = await _azure.NetworkInterfaces.GetByIdAsync(nicId, cancellationToken);

            if (nic == null)
            {
                throw NotFoundException.CreateForAzureResourceById(nicId);
            }

            return CreateNicDto(nic);
        }

        async Task<VmDiskDto> CreateDiskDto(string diskId, bool isOs, CancellationToken cancellationToken)
        {
            var disk = await _azure.Disks.GetByIdAsync(diskId, cancellationToken);

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
