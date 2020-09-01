using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service;
using System;

namespace Sepes.Infrastructure.Util
{
    //Finds correct service based on the resource type
    public static class AzureResourceServiceResolver
    { 
        //List<IHasProvisioningState> services
        public static IHasProvisioningState GetServiceWithProvisioningState(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVMService>(),
            _ => null,
        };

        public static IHasTags GetServiceWithTags(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVMService>(),
            _ => null,
        };

        public static IHasExists GetServiceWithExistance(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVMService>(),
            _ => null,
        };
    }
}
