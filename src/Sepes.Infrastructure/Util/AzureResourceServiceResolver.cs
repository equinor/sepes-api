using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Service;
using System;

namespace Sepes.Infrastructure.Util
{
    //Finds correct service based on the resource type
    public static class AzureResourceServiceResolver
    {
        public static class ResourceTypes
        {
            public const string ResourceGroup = "ResourceGroup";
            public const string VirtualNetwork = "Microsoft.Network/virtualNetworks";
            public const string NetworkSecurityGroup = "Microsoft.Network/networkSecurityGroups";
            public const string StorageAccount = "Microsoft.Storage/storageAccounts";
            //TODO: Change remaining typeNames to valid Azure Types.
            public const string Bastion = "Bastion";
            public const string VirtualMachine = "VirtualMachine";
        }

        //List<IHasProvisioningState> services
        public static IHasProvisioningState GetServiceWithProvisioningState(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            ResourceTypes.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            ResourceTypes.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            ResourceTypes.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
            ResourceTypes.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            ResourceTypes.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            ResourceTypes.VirtualMachine => serviceProvider.GetRequiredService<IAzureVMService>(),
            _ => null,
        };

        public static IHasTags GetServiceWithTags(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            ResourceTypes.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            ResourceTypes.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            ResourceTypes.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
            ResourceTypes.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            ResourceTypes.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            ResourceTypes.VirtualMachine => serviceProvider.GetRequiredService<IAzureVMService>(),
            _ => null,
        };

        public static IHasExists GetServiceWithExistance(IServiceProvider serviceProvider, string resourceType) => resourceType switch
        {
            ResourceTypes.ResourceGroup => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
            ResourceTypes.VirtualNetwork => serviceProvider.GetRequiredService<IAzureVNetService>(),
            ResourceTypes.NetworkSecurityGroup => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
            ResourceTypes.StorageAccount => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
            ResourceTypes.Bastion => serviceProvider.GetRequiredService<IAzureBastionService>(),
            ResourceTypes.VirtualMachine => serviceProvider.GetRequiredService<IAzureVMService>(),
            _ => null,
        };
    }
}
