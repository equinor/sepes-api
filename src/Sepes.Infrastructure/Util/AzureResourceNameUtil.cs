using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    // Naming prefixes should follow these conventions https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging.
    public static class AzureResourceNameUtil
    {
        public const string AZURE_RESOURCE_INITIAL_ID_OR_NAME = "n/a";

        public static string ResourceGroup(string studyName, string sandboxName)
        {
            return AzureResourceNameConstructor("rg-study-", studyName, sandboxName, maxLength: 64, addUniqueEnding: true);
        }

        public static string VNet(string studyName, string sandboxName) => AzureResourceNameConstructor("vnet-", studyName, sandboxName);


        public static string SubNet(string studyName, string sandboxName) => AzureResourceNameConstructor("snet-", studyName, sandboxName);

        public static string NetworkSecGroup(string studyName, string sandboxName) => AzureResourceNameConstructor("nsg-", studyName, sandboxName);

        public static string NetworkSecGroupSubnet(string studyName, string sandboxName) => AzureResourceNameConstructor("nsg-snet-", studyName, sandboxName);

        public static string Bastion(string studyName, string sandboxName) => AzureResourceNameConstructor("bastion-", studyName, sandboxName);

        public static string BastionPublicIp(string bastionName) => EnsureMaxLength($"pip-{bastionName}", 64);

        // Storage account names must be between 3 and 24 characters in length and may contain numbers and lowercase letters only.
        // Your storage account name must be unique within Azure. No two storage accounts can have the same name.
        // StorageAccount names needs to be unique in Azure scope.
        public static string StorageAccount(string sandboxName)
        {
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            sandboxName = MakeStringAlphanumericAndRemoveWhitespace(sandboxName);
            if (sandboxName.Length > 18)
            {
                return $"st{sandboxName.ToLower().Substring(0, 18)}{shortGuid}";
            }
            return $"st{sandboxName.ToLower()}{shortGuid}";
        }

        public static string EnsureMaxLength(string potentialName, int maxLength = 0)
        {
            if (maxLength > 0 && potentialName.Length > maxLength)
            {
                return potentialName.Substring(0, maxLength);
            }

            return potentialName;
        }

        public static string DiagnosticsStorageAccount(string studyName, string sandboxName)
        {
            var studyNameNormalized = MakeStringAlphanumericAndRemoveWhitespace(studyName);
            var sanboxNameNormalized = MakeStringAlphanumericAndRemoveWhitespace(sandboxName);

            return AzureResourceNameConstructor("stdiag", studyNameNormalized, sanboxNameNormalized, maxLength: 24, addUniqueEnding: true, avoidDash: true);
        }

        public static string VirtualMachine(string studyName, string sandboxName, string userSuffix)
        {

            var studyNameNormalized = MakeStringAlphanumericAndRemoveWhitespace(studyName, 10);
            var sanboxNameNormalized = MakeStringAlphanumericAndRemoveWhitespace(sandboxName, 10);
            var userSuffixNormalized = MakeStringAlphanumericAndRemoveWhitespace(userSuffix);

            var partWithoutUserSuffix = AzureResourceNameConstructor("vm-", studyNameNormalized, sanboxNameNormalized, maxLength: 64 - userSuffixNormalized.Length, addUniqueEnding: false, avoidDash: false);

            return $"{partWithoutUserSuffix}-{userSuffixNormalized}";

        }

        public const string NSG_RULE_FOR_VM_PREFIX = "vm-rule-";

        public static string NsgRuleNameForVm(int vmId, string suffix = null)
        {
            //The name must begin with a letter or number, end with a letter, number or underscore, and may contain only letters, numbers, underscores, periods, or hyphens.
            //Max 80 characters

            var vmIdString = vmId.ToString();
            var suffixMaxLength = 80 - NSG_RULE_FOR_VM_PREFIX.Length - vmIdString.Length - 1;


            var suffixNormalized = "";

            if (suffix == null)
            {
                suffixNormalized = Normalize(Guid.NewGuid().ToString(), suffixMaxLength);
            }
            else
            {
                suffixNormalized = Normalize(suffix, suffixMaxLength);
            }

            return $"{NSG_RULE_FOR_VM_PREFIX}{vmId}-{suffixNormalized}";
        }

        public static string AzureResourceNameConstructor(string prefix, string studyName, string sandboxName, int maxLength = 64, bool addUniqueEnding = false, bool avoidDash = false)
        {
            var shortUniquePart = addUniqueEnding ? (avoidDash ? "" : "-") + Guid.NewGuid().ToString().ToLower().Substring(0, 3) : "";
            var availableSpaceForStudyAndSanboxName = maxLength - prefix.Length - shortUniquePart.Length - (avoidDash ? 0 : 1);

            var alphanumericStudyName = MakeStringAlphanumericAndRemoveWhitespace(studyName);
            var alphanumericSandboxName = MakeStringAlphanumericAndRemoveWhitespace(sandboxName);

            var charachtersLeft = availableSpaceForStudyAndSanboxName - (alphanumericStudyName.Length + alphanumericSandboxName.Length);

            if (charachtersLeft < 0)
            {
                var totalTrim = Math.Abs(charachtersLeft);

                var nameLengthDiff = alphanumericStudyName.Length - alphanumericSandboxName.Length;
                var amountToTrimOff = Math.Abs(nameLengthDiff) > totalTrim ? totalTrim : Math.Abs(nameLengthDiff);

                if (nameLengthDiff > 0) // study name is longer, trim it down to sandbox length
                {
                    alphanumericStudyName = alphanumericStudyName.Substring(0, alphanumericStudyName.Length - amountToTrimOff);
                }
                else // sandbox name is longer, trim it down to study length
                {
                    alphanumericSandboxName = alphanumericSandboxName.Substring(0, alphanumericSandboxName.Length - amountToTrimOff);
                }

                //Both names are now equal in length, now we can equally remove from both

                charachtersLeft = availableSpaceForStudyAndSanboxName - (alphanumericStudyName.Length + alphanumericSandboxName.Length);

                if (charachtersLeft < 0)
                {
                    var mustRemoveEach = Math.Abs(charachtersLeft) / 2;
                    var even = charachtersLeft % 2 == 0;
                    alphanumericStudyName = alphanumericStudyName.Substring(0, alphanumericStudyName.Length - mustRemoveEach - (even ? 0 : 1));
                    alphanumericSandboxName = alphanumericSandboxName.Substring(0, alphanumericSandboxName.Length - mustRemoveEach);
                }
            }

            return $"{prefix}{alphanumericStudyName}{(avoidDash ? "" : "-")}{alphanumericSandboxName}{shortUniquePart}";
        }

        static string Normalize(string input, int limit = 0)
        {
            var normalizedString = StripWhitespace(input).ToLower();

            return EnsureMaxLength(normalizedString, limit);
        }

        public static string MakeStringAlphanumericAndRemoveWhitespace(string str, int limit = 0)
        {

            var alphaNummericString = new string((from c in str
                                                  where char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != 'æ' && c != 'ø' && c != 'å'
                                                  select c).ToArray()).ToLower();

            return EnsureMaxLength(alphaNummericString, limit);
        }

        public static string StripWhitespace(string str) => new string((from c in str
                                                                        where !char.IsWhiteSpace(c) && c != 'æ' && c != 'ø' && c != 'å'
                                                                        select c
                                                                ).ToArray());


    }
}
