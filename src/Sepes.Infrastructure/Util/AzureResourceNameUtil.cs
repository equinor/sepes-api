using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceNameUtil
    {
        public static string VNet(string studyName, string sandboxName)
        {
            return $"vnet-study-{studyName}-{sandboxName}";
        }

        public static string Sandbox(string studyName)
        {
            return $"{studyName}-sandbox";
        }

        public static string NetworkSecGroup(string sandboxName)
        {
            return $"nsg-{sandboxName}";
        }

        public static string NetworkSecGroupSubnet(string sandboxName)
        {
            return $"nsg-snet-{sandboxName}";
        }

        public static string Bastion(string studyName, string sandboxName)
        {
            return $"bastion-{studyName}-{sandboxName}";
        }

        public static string BastionPublicIp(string studyName, string sandboxName)
        {
            return $"pip-{studyName}-{sandboxName}-bastion";
        }
    }
}
