using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
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
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNetworkSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVmService>(),
            _ => null,
        };

        public static IHasTags GetServiceWithTags(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNetworkSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVmService>(),
            _ => null,
        };

        public static IPerformCloudResourceCRUD GetCRUDService(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            AzureResourceType.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            AzureResourceType.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNetworkSecurityGroupService>(),
            AzureResourceType.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            AzureResourceType.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            AzureResourceType.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            AzureResourceType.VirtualMachine => serviceProvider.GetRequiredService<IAzureVmService>(),
            _ => null,
        };
    }
}
