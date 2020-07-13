﻿namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceNameUtil
    {

        public static string ResourceGroupForStudy(string sandboxName)
        {
            return $"rg-study-{sandboxName}";
        }
        public static string Sandbox(string studyName)
        {
            return $"{studyName}-sandbox";
        }

        public static string VNet(string studyName, string sandboxName)
        {
            return $"vnet-study-{sandboxName}";
        }

        public static string SubNet(string sandboxName)
        {
            return $"snet-{sandboxName}";
        }

        public static string NetworkSecGroup(string sandboxName)
        {
            return $"nsg-{sandboxName}";
        }

        public static string NetworkSecGroupSubnet(string sandboxName)
        {
            return $"nsg-snet-{sandboxName}";
        }

        public static string Bastion(string sandboxName)
        {
            return $"bastion-{sandboxName}";
        }

        public static string BastionPublicIp(string sandboxName)
        {
            return $"pip-{sandboxName}-bastion";
        }

        public static string StorageAccount(string sandboxName)
        {
            return $"{sandboxName.ToLower()}-storage";
        }

        public static string VirtualMachine(string sandboxName)
        {
            return $"VM-{sandboxName}";
        }
    }
}
