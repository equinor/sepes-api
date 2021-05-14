using Microsoft.Extensions.DependencyInjection;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Interface.Service;
using System;

namespace Sepes.Azure.Util
{
    //Finds correct service based on the resource type
    public static class AzureResourceServiceResolver
    {
        //List<IHasProvisioningState> services
        public static IHasProvisioningState GetServiceWithProvisioningState(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVirtualNetworkService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNetworkSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVirtualMachineService>(),
            _ => null,
        };

        public static IHasTags GetServiceWithTags(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVirtualNetworkService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNetworkSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVirtualMachineService>(),
            _ => null,
        };

        public static IPerformResourceProvisioning GetProvisioningService(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNetworkSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVirtualNetworkService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVirtualMachineService>(),
            _ => null,
        };

        public static IPerformResourceProvisioning GetProvisioningServiceOrThrow(IServiceProvider serviceProvider, string resourceType)
        {
            var service = GetProvisioningService(serviceProvider, resourceType);

            if (service == null)
            {
                throw new NullReferenceException($"Unable to resolve provisioning service for type {resourceType}");
            }

            return service;
        }

        public static IHasCorsRules GetCorsRuleServiceOrThrow(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {           
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountCorsRuleService>(),    
            _ => throw new NullReferenceException($"Unable to cors provisioning service for type {resourceType}"),
        };

        public static IHasFirewallRules GetFirewallRuleService(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountNetworkRuleService>(),
            _ => throw new NullReferenceException($"Unable to resolve firewall service for type {resourceType}"),
        };

    }
}

