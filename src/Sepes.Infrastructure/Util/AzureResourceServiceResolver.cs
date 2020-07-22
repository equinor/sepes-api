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

            switch (resourceType)
            {
                case "ResourceGroup":
                    return serviceProvider.GetRequiredService<IAzureResourceGroupService>();
                case "Network":
                    return serviceProvider.GetRequiredService<IAzureVNetService>();
                case "NetworkSecurityGroup":
                    return serviceProvider.GetRequiredService<IAzureNwSecurityGroupService>();
                case "StorageAccount":
                    return serviceProvider.GetRequiredService<IAzureStorageAccountService>();
                case "Virtualmachine":
                    return serviceProvider.GetRequiredService<IAzureVMService>();                          
                case "Bastion":
                    return serviceProvider.GetRequiredService<IAzureBastionService>();
                default: return null;
            }
        }
    }
}
