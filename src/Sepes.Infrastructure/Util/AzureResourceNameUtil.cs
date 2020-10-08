using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    // Naming prefixes should follow these conventions https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging.
    public static class AzureResourceNameUtil
    {
        public const string AZURE_RESOURCE_INITIAL_NAME = "n/a";

        public static string ResourceGroup(string studyName, string sandboxName)
        {
            //var shortUniquePart = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            ////Max length is 
            //var resourceGroupPrefix = ;
            //var availableSpaceForStudyAndSanboxName = 64 - resourceGroupPrefix.Length - shortUniquePart.Length - 1;
            //var normalizedStudyName = Normalize(studyName);
            //var normalizedSanboxName = Normalize(sandboxName);

            //var charachtersLeft = availableSpaceForStudyAndSanboxName - (normalizedStudyName.Length + normalizedSanboxName.Length);

            //if (charachtersLeft < 0)
            //{
            //    normalizedStudyName = normalizedStudyName.Substring(0, normalizedStudyName.Length - (Math.Abs(charachtersLeft / 2)));
            //    normalizedSanboxName = normalizedSanboxName.Substring(0, normalizedSanboxName.Length - (Math.Abs(charachtersLeft / 2)));
            //}

            //return $"{resourceGroupPrefix}{normalizedStudyName}-{normalizedSanboxName}-{shortUniquePart}";

            return AzureResourceNameConstructor("rg-study-", studyName, sandboxName, maxLength: 64, addUniqueEnding: true);
        }

        public static string VNet(string studyName, string sandboxName) => AzureResourceNameConstructor("vnet-", studyName, sandboxName);


        public static string SubNet(string studyName, string sandboxName) => AzureResourceNameConstructor("snet-", studyName, sandboxName);

        public static string NetworkSecGroup(string studyName, string sandboxName) => AzureResourceNameConstructor("nsg-", studyName, sandboxName);

        public static string NetworkSecGroupSubnet(string studyName, string sandboxName) => AzureResourceNameConstructor("nsg-snet-", studyName, sandboxName);

        public static string Bastion(string studyName, string sandboxName) => AzureResourceNameConstructor("bastion-", studyName, sandboxName);

        public static string BastionPublicIp(string studyName, string sandboxName) => AzureResourceNameConstructor("pip-bastion-", studyName, sandboxName);

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

        public static string DiagnosticsStorageAccount(string studyName, string sandboxName)
        {
            var studyNameNormalized = MakeStringAlphanumeric(studyName);
            var sanboxNameNormalized = MakeStringAlphanumeric(sandboxName);

            return AzureResourceNameConstructor("stdiag", studyNameNormalized, sanboxNameNormalized, maxLength: 24, addUniqueEnding:true, avoidDash: true);
        }

        public static string VirtualMachine(string sandboxName) => StripWhitespace($"vm-{sandboxName}");


        public static string AzureResourceNameConstructor(string prefix, string studyName, string sandboxName, int maxLength = 64, bool addUniqueEnding = false, bool avoidDash = false)
        {
            var shortUniquePart = addUniqueEnding ? (avoidDash ? "" : "-") + Guid.NewGuid().ToString().ToLower().Substring(0, 3) : "";
            var availableSpaceForStudyAndSanboxName = maxLength - prefix.Length - shortUniquePart.Length - (avoidDash ? 0 : 1);

            var normalizedStudyName = Normalize(studyName);
            var normalizedSanboxName = Normalize(sandboxName);

            var charachtersLeft = availableSpaceForStudyAndSanboxName - (normalizedStudyName.Length + normalizedSanboxName.Length);

            if (charachtersLeft < 0)
            {
                var totalTrim = Math.Abs(charachtersLeft);

                var nameLengthDiff = normalizedStudyName.Length - normalizedSanboxName.Length;
                var amountToTrimOff = Math.Abs(nameLengthDiff) > totalTrim ? totalTrim : Math.Abs(nameLengthDiff);

                if (nameLengthDiff > 0) // study name is longer, trim it down to sandbox length
                {
                    normalizedStudyName = normalizedStudyName.Substring(0, normalizedStudyName.Length - amountToTrimOff);
                }
                else // sandbox name is longer, trim it down to study length
                {
                    normalizedSanboxName = normalizedSanboxName.Substring(0, normalizedSanboxName.Length - amountToTrimOff);
                }

               //Both names are now equal in length, now we can equally remove from both

                charachtersLeft = availableSpaceForStudyAndSanboxName - (normalizedStudyName.Length + normalizedSanboxName.Length);

                if (charachtersLeft < 0)
                {
                    var mustRemoveEach = Math.Abs(charachtersLeft) / 2;
                    var even = charachtersLeft % 2 == 0;
                    normalizedStudyName = normalizedStudyName.Substring(0, normalizedStudyName.Length - mustRemoveEach - (even ? 0 : 1));
                    normalizedSanboxName = normalizedSanboxName.Substring(0, normalizedSanboxName.Length - mustRemoveEach);
                }
            }

            return $"{prefix}{normalizedStudyName}{(avoidDash ? "" : "-")}{normalizedSanboxName}{shortUniquePart}";
        }

        static string Normalize(string input)
        {
            return StripWhitespace(input).ToLower();
        }


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
