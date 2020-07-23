using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Service;
using System;

namespace Sepes.Infrastructure.Util
{
    //Finds correct service based on the resource type
    public static class AzureResourceServiceResolver
    {
        //List<IHasProvisioningState> services
        public static IHasProvisioningState GetServiceWithProvisioningState(IServiceProvider serviceProvider, string resourceType)
        {

            return resourceType switch
            {
                "ResourceGroup" => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
                "Microsoft.Network/virtualNetworks" => serviceProvider.GetRequiredService<IAzureVNetService>(),
                "Microsoft.Network/networkSecurityGroups" => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
                "Microsoft.Storage/storageAccounts" => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
                //TODO: Change remaining typeNames to valid Azure Types..
                "Virtualmachine" => serviceProvider.GetRequiredService<IAzureVMService>(),
                "Bastion" => serviceProvider.GetRequiredService<IAzureBastionService>(),
                _ => null,
            };
        }

        public static IHasTags GetServiceWithTags(IServiceProvider serviceProvider, string resourceType)
        {
            return resourceType switch
            {
                "ResourceGroup" => serviceProvider.GetRequiredService<IAzureResourceGroupService>(),
                "Microsoft.Network/virtualNetworks" => serviceProvider.GetRequiredService<IAzureVNetService>(),
                "Microsoft.Network/networkSecurityGroups" => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
                "Microsoft.Storage/storageAccounts" => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
                //TODO: Change remaining typeNames to valid Azure Types.
                "Virtualmachine" => serviceProvider.GetRequiredService<IAzureVMService>(),
                "Bastion" => serviceProvider.GetRequiredService<IAzureBastionService>(),
                _ => null,
            };
        }
    }
}
