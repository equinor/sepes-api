using AutoMapper;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureVmService : AzureServiceBase, IAzureVmService
    {
        readonly IAzureNetworkSecurityGroupService _nsgService;
        readonly IMapper _mapper;

        public AzureVmService(IConfiguration config, ILogger<AzureVmService> logger, IAzureNetworkSecurityGroupService nsgService, IMapper mapper)
            : base(config, logger)
        {
            _nsgService = nsgService;
            _mapper = mapper;
        }

        public async Task<CloudResourceCRUDResult> EnsureCreated(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Creating VM: {parameters.Name} in resource Group: {parameters.ResourceGroupName}");

            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(parameters.ConfigurationString);

            var passwordReference = vmSettings.Password;
            string password = await GetPasswordFromKeyVault(passwordReference);

            string vmSize = vmSettings.Size;

            var createdVm = await Create(parameters.Region,
                parameters.ResourceGroupName,
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

                    await ApplyVmDataDisks(parameters.ResourceGroupName, parameters.Name, sizeAsInt);
                }
            }

            var primaryNic = await _azure.NetworkInterfaces.GetByIdAsync(createdVm.PrimaryNetworkInterfaceId, cancellationToken);

            await UpdateVmRules(parameters, vmSettings, primaryNic.PrimaryPrivateIP, cancellationToken);

            var result = CreateCRUDResult(createdVm);

            await DeletePasswordFromKeyVault(passwordReference);

            _logger.LogInformation($"Done creating Network Security Group for sandbox with Id: {parameters.SandboxId}! Id: {createdVm.Id}");
            return result;
        }
       

        public async Task<CloudResourceCRUDResult> Update(CloudResourceCRUDInput parameters, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Updating VM {parameters.Name}");

            var vm = await GetAsync(parameters.ResourceGroupName, parameters.Name);
            var primaryNic = await _azure.NetworkInterfaces.GetByIdAsync(vm.PrimaryNetworkInterfaceId, cancellationToken);

            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(parameters.ConfigurationString);

            await UpdateVmRules(parameters, vmSettings, primaryNic.PrimaryPrivateIP, cancellationToken);

            var result = CreateCRUDResult(vm);

            return result;
        }

        async Task UpdateVmRules(CloudResourceCRUDInput parameters, VmSettingsDto vmSettings, string privateIp, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Setting desired VM rules for {parameters.Name}");

            var existingRules = await _nsgService.GetNsgRulesContainingName(parameters.ResourceGroupName, parameters.NetworkSecurityGroupName, $"{AzureResourceNameUtil.NSG_RULE_FOR_VM_PREFIX}{parameters.DatabaseId}", cancellationToken);
            var existingRulesThatStillExists = new HashSet<string>();

            if (vmSettings.Rules == null)
            {
                throw new Exception($"No rules exists for VM {parameters.Name}");
            }
            else
            {
                foreach (var curRule in vmSettings.Rules)
                {
                    try
                    {
                        var ruleMapped = _mapper.Map<NsgRuleDto>(curRule);

                        if (curRule.Direction == RuleDirection.Inbound)
                        {
                            ruleMapped.SourceAddress = curRule.Ip;
                            ruleMapped.SourcePort = curRule.Port;
                            ruleMapped.DestinationAddress = privateIp;
                            ruleMapped.DestinationPort = curRule.Port;

                            //get existing rule and use that name
                            if (existingRules.ContainsKey(curRule.Name))
                            {
                                existingRulesThatStillExists.Add(curRule.Name);
                                await _nsgService.UpdateInboundRule(parameters.ResourceGroupName, parameters.NetworkSecurityGroupName, ruleMapped, cancellationToken);
                            }
                            else
                            {
                                await _nsgService.AddInboundRule(parameters.ResourceGroupName, parameters.NetworkSecurityGroupName, ruleMapped, cancellationToken);
                            }

                        }
                        else
                        {
                            ruleMapped.SourceAddress = privateIp;
                            ruleMapped.SourcePort = curRule.Port;

                            if (ruleMapped.Name.Contains(AzureVmConstants.RulePresets.OPEN_CLOSE_INTERNET))
                            {
                                ruleMapped.DestinationAddress = "*";
                                ruleMapped.DestinationPort = 0;
                            }
                            else
                            {
                                ruleMapped.DestinationAddress = curRule.Ip;
                                ruleMapped.DestinationPort = curRule.Port;
                            }

                            if (existingRules.ContainsKey(curRule.Name))
                            {
                                existingRulesThatStillExists.Add(curRule.Name);
                                await _nsgService.UpdateOutboundRule(parameters.ResourceGroupName, parameters.NetworkSecurityGroupName, ruleMapped, cancellationToken);
                            }
                            else
                            {
                                await _nsgService.AddOutboundRule(parameters.ResourceGroupName, parameters.NetworkSecurityGroupName, ruleMapped, cancellationToken);
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Unable to create rule {curRule.Name} for VM {parameters.Name}", ex);
                    }
                }
            }

            if (existingRules != null && existingRules.Count > 0)
            {
                foreach (var curExistingKvp in existingRules)
                {
                    if (!existingRulesThatStillExists.Contains(curExistingKvp.Key))
                    {
                        await _nsgService.DeleteRule(parameters.ResourceGroupName, parameters.NetworkSecurityGroupName, curExistingKvp.Key, cancellationToken);
                    }
                }
            }

            _logger.LogInformation($"Done setting desired VM rules for {parameters.Name}");
        }

        public async Task<CloudResourceCRUDResult> GetSharedVariables(CloudResourceCRUDInput parameters)
        {
            var vm = await GetAsync(parameters.ResourceGroupName, parameters.Name);
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
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVmOperatingSystemConstants.Windows.Server2019DataCenter.Publisher, AzureVmOperatingSystemConstants.Windows.Server2019DataCenter.Offer, AzureVmOperatingSystemConstants.Windows.Server2019DataCenter.Sku);
                    break;
                case "win2019datacentercore":
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVmOperatingSystemConstants.Windows.Server2019DataCenterCore.Publisher, AzureVmOperatingSystemConstants.Windows.Server2019DataCenterCore.Offer, AzureVmOperatingSystemConstants.Windows.Server2019DataCenterCore.Sku);
                    break;
                case "win2016datacenter":
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVmOperatingSystemConstants.Windows.Server2016DataCenter.Publisher, AzureVmOperatingSystemConstants.Windows.Server2016DataCenter.Offer, AzureVmOperatingSystemConstants.Windows.Server2016DataCenter.Sku);
                    break;
                case "win2016datacentercore":
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVmOperatingSystemConstants.Windows.Server2016DataCenterCore.Publisher, AzureVmOperatingSystemConstants.Windows.Server2016DataCenterCore.Offer, AzureVmOperatingSystemConstants.Windows.Server2016DataCenterCore.Sku);
                    break;
                default:
                    _logger.LogInformation("Could not match distro argument. Default will be chosen: Windows Server 2019");
                    withOS = vmCreatable.WithLatestWindowsImage(AzureVmOperatingSystemConstants.Windows.Server2019DataCenter.Publisher, AzureVmOperatingSystemConstants.Windows.Server2019DataCenter.Offer, AzureVmOperatingSystemConstants.Windows.Server2019DataCenter.Sku);
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
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVmOperatingSystemConstants.Linux.UbuntuServer1804LTS.Publisher, AzureVmOperatingSystemConstants.Linux.UbuntuServer1804LTS.Offer, AzureVmOperatingSystemConstants.Linux.UbuntuServer1804LTS.Sku);
                    break;
                case "ubuntu16lts":
                    withOS = vmCreatable.WithPopularLinuxImage(KnownLinuxVirtualMachineImage.UbuntuServer16_04_Lts);
                    break;
                case "rhel":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVmOperatingSystemConstants.Linux.RedHat7LVM.Publisher, AzureVmOperatingSystemConstants.Linux.RedHat7LVM.Offer, AzureVmOperatingSystemConstants.Linux.RedHat7LVM.Sku);
                    break;
                case "debian":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVmOperatingSystemConstants.Linux.Debian10.Publisher, AzureVmOperatingSystemConstants.Linux.Debian10.Offer, AzureVmOperatingSystemConstants.Linux.Debian10.Sku);
                    break;
                case "centos":
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVmOperatingSystemConstants.Linux.CentOS75.Publisher, AzureVmOperatingSystemConstants.Linux.CentOS75.Offer, AzureVmOperatingSystemConstants.Linux.CentOS75.Sku);
                    break;
                default:
                    _logger.LogInformation("Could not match distro argument. Default will be chosen: Ubuntu 18.04-LTS");
                    withOS = vmCreatable.WithLatestLinuxImage(AzureVmOperatingSystemConstants.Linux.UbuntuServer1804LTS.Publisher, AzureVmOperatingSystemConstants.Linux.UbuntuServer1804LTS.Offer, AzureVmOperatingSystemConstants.Linux.UbuntuServer1804LTS.Sku);
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
                await Delete(parameters.ResourceGroupName, parameters.Name);

                //Also remember to delete osdisk
                provisioningState = await GetProvisioningState(parameters.ResourceGroupName, parameters.Name);

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

     

        public async Task<VmExtendedDto> GetExtendedInfo(string resourceGroupName, string resourceName, CancellationToken cancellationToken = default)
        {
            var vm = await GetAsync(resourceGroupName, resourceName);

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
                _logger.LogWarning($"Unable to fetch public IP settings for VM {vm.Name}");
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
