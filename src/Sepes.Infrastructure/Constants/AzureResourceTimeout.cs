using System;

namespace Sepes.Infrastructure.Constants
{
    public class AzureResourceTimeout
    {
        public static int RESOURCE_GROUP = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        public static int STORAGE_ACCOUNT = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        public static int VNet = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        public static int NSG = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        public static int BASTION = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
        public static int VM = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
    }
}
