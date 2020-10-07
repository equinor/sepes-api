using Sepes.Infrastructure.Constants;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceProivisoningTimeoutResolver
    {
        public static int GetTimeoutForOperationInSeconds(string resourceType, string operationType)
        {
            if(resourceType == AzureResourceType.StorageAccount)
            {
                return 180;
            }
            else if (resourceType == AzureResourceType.NetworkSecurityGroup)
            {
                return 180;
            }
            else if (resourceType == AzureResourceType.VirtualNetwork)
            {
                return 180;
            }
            else if (resourceType == AzureResourceType.ResourceGroup)
            {
                if(operationType == CloudResourceOperationType.CREATE)
                {
                    return 60;
                }
                else if (operationType == CloudResourceOperationType.CREATE)
                {
                    return 600;
                }


            }
            else if (resourceType == AzureResourceType.Bastion)
            {
                return 600;
            }

            return 60;
        }
    }
}
