using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceTypeUtil
    {
       public static string GetUserFriendlyName(SandboxResource resource)
        {
            var resourceType = resource.ResourceType;

            switch (resourceType)
            {
                case AzureResourceType.ResourceGroup:
                    return "Resource Group";
                case AzureResourceType.StorageAccount:
                    return "Storage Account";
                case AzureResourceType.VirtualNetwork:
                    return "Virtual Network";
                case AzureResourceType.NetworkSecurityGroup:
                    return "Network Security Group";
                case AzureResourceType.Bastion:
                    return "Bastion";
                case AzureResourceType.VirtualMachine:
                    return "Virtual Machine";
                default:
                    return "n/a";
            }
        }
    }
}
