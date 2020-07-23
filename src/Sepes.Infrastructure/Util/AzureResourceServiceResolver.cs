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
                "Network" => serviceProvider.GetRequiredService<IAzureVNetService>(),
                "NetworkSecurityGroup" => serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>(),
                "StorageAccount" => serviceProvider.GetRequiredService<IAzureStorageAccountService>(),
                "Virtualmachine" => serviceProvider.GetRequiredService<IAzureVMService>(),
                "Bastion" => serviceProvider.GetRequiredService<IAzureBastionService>(),
                _ => null,
            };
        }
    }
}
