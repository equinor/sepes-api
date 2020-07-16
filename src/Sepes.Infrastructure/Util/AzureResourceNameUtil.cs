using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    // Naming prefixes should follow these conventions https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging.
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

        // Name gets to long. Need to think of something smarter...
        // Storage account names must be between 3 and 24 characters in length and may contain numbers and lowercase letters only.
        // Your storage account name must be unique within Azure.No two storage accounts can have the same name.
        // Current solution might possibly allow for duplicate names.
        // StorageAccount names needs to be unique in Azure scope.
        public static string StorageAccount(string sandboxName)
        {
            sandboxName = MakeStringAlphanumeric(sandboxName);
            if (sandboxName.Length > 21)
            {
                return $"st{sandboxName.ToLower().Substring(0, 21)}";
            }
            return $"st{sandboxName.ToLower()}";
        }

        public static string DiagnosticsStorageAccount(string sandboxName)
        {
            sandboxName = MakeStringAlphanumeric(sandboxName);
            if (sandboxName.Length > 17)
            {
                return $"stdiag{sandboxName.ToLower().Substring(0, 17)}";
            }
            return $"stdiag{sandboxName.ToLower()}";
        }

        public static string VirtualMachine(string sandboxName)
        {
            return $"vm-{sandboxName}";
        }

        static string MakeStringAlphanumeric(string str)
        {
            return new string((from c in str
                              where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                              select c
                              ).ToArray());
        }
    }
}
