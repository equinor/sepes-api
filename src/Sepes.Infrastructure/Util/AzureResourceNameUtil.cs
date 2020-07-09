namespace Sepes.Infrastructure.Util
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

        public static string BastionPublicIp(string sandboxName)
        {
            return $"pip-{sandboxName}-bastion";
        }
    }
}
