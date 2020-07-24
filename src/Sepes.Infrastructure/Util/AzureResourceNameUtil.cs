using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    // Naming prefixes should follow these conventions https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging.
    public static class AzureResourceNameUtil
    {
        public static string ResourceGroup(string sandboxName)
        {
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            return StripWhitespace($"rg-study-{sandboxName}{shortGuid}");
        }
        public static string Sandbox(string studyName) => StripWhitespace($"{studyName}-sandbox");

        public static string VNet(string studyName, string sandboxName) => StripWhitespace($"vnet-study-{sandboxName}");

        public static string SubNet(string sandboxName) => StripWhitespace($"snet-{sandboxName}");

        public static string NetworkSecGroup(string sandboxName) => StripWhitespace($"nsg-{sandboxName}");

        public static string NetworkSecGroupSubnet(string sandboxName) => StripWhitespace($"nsg-snet-{sandboxName}");

        public static string Bastion(string sandboxName) => StripWhitespace($"bastion-{sandboxName}");

        public static string BastionPublicIp(string sandboxName) => StripWhitespace($"pip-{sandboxName}-bastion");

        // Storage account names must be between 3 and 24 characters in length and may contain numbers and lowercase letters only.
        // Your storage account name must be unique within Azure. No two storage accounts can have the same name.
        // StorageAccount names needs to be unique in Azure scope.
        public static string StorageAccount(string sandboxName)
        {
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            sandboxName = MakeStringAlphanumeric(sandboxName);
            if (sandboxName.Length > 18)
            {
                return $"st{sandboxName.ToLower().Substring(0, 18)}{shortGuid}";
            }
            return $"st{sandboxName.ToLower()}{shortGuid}";
        }

        public static string DiagnosticsStorageAccount(string sandboxName)
        {
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            sandboxName = MakeStringAlphanumeric(sandboxName);
            if (sandboxName.Length > 14)
            {
                return $"stdiag{sandboxName.ToLower().Substring(0, 14)}{shortGuid}";
            }
            return $"stdiag{sandboxName.ToLower()}{shortGuid}";
        }

        public static string VirtualMachine(string sandboxName) => StripWhitespace($"vm-{sandboxName}");

        static string MakeStringAlphanumeric(string str) => new string((from c in str
                                                                        where char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)
                                                                        select c
                                                                       ).ToArray());

        static string StripWhitespace(string str) => new string((from c in str
                                                                 where !char.IsWhiteSpace(c)
                                                                 select c
                                                                ).ToArray());
    }
}
