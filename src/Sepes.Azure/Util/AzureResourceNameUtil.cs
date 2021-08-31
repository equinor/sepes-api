using System;
using System.Linq;

namespace Sepes.Azure.Util
{
    // Naming prefixes should follow these conventions https://docs.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/naming-and-tagging.
    public static class AzureResourceNameUtil
    {
        public const string AZURE_RESOURCE_INITIAL_ID_OR_NAME = "n/a";

        public static string SandboxResourceGroup(string studyName, string sandboxName)
        {
            return AzureResourceNameConstructor("rg-study-", studyName, sandboxName, maxLength: 64, addUniqueEnding: true);
        }

        public static string StudySpecificDatasetResourceGroup(string studyName)
        {
            return AzureResourceNameConstructor("rg-study-", studyName, null, "-datasets", maxLength: 64, addUniqueEnding: true);
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
        public static string StudySpecificDataSetStorageAccount(string datasetName)
        {
            string prefix = "stds";
            var uniquePart = Guid.NewGuid().ToString().ToLower().Substring(0, 5);

            var nameNormalized = RemoveSpecialCharactersAndRemoveWhitespace(datasetName, true, 24 - prefix.Length - uniquePart.Length);

            return $"{prefix}{nameNormalized}{uniquePart}";
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
            var studyNameNormalized = RemoveSpecialCharactersAndRemoveWhitespace(studyName);
            var sanboxNameNormalized = RemoveSpecialCharactersAndRemoveWhitespace(sandboxName);

            return AzureResourceNameConstructor("stdiag", studyNameNormalized, sanboxNameNormalized, maxLength: 24, addUniqueEnding: true, avoidDash: true);
        }

        public static string VirtualMachinePublicIp(string vmName) => EnsureMaxLength($"pip-{vmName}", 64);

        public static string VirtualMachine(string studyName, string sandboxName, string userSuffix)
        {

            var studyNameNormalized = RemoveSpecialCharactersAndRemoveWhitespace(studyName, limit: 10);
            var sanboxNameNormalized = RemoveSpecialCharactersAndRemoveWhitespace(sandboxName, limit: 10);
            var userSuffixNormalized = RemoveSpecialCharactersAndRemoveWhitespace(userSuffix);

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


            var suffixNormalized = suffix == null ?
                StripWhitespaceAndEnsureLength(Guid.NewGuid().ToString(), suffixMaxLength) :
                StripWhitespaceAndEnsureLength(suffix, suffixMaxLength);

            return $"{NSG_RULE_FOR_VM_PREFIX}{vmId}-{suffixNormalized}";
        }

      

        public static string AzureResourceNameConstructor(string prefix, string studyName, string sandboxName = null, string suffix = null, int maxLength = 64, bool addUniqueEnding = false, bool avoidDash = false)
        {
            var prefixLength = prefix.Length;
            var suffixLength = String.IsNullOrWhiteSpace(suffix) ? 0 : suffix.Length;
            var shortUniquePart = addUniqueEnding ? (avoidDash ? "" : "-") + Guid.NewGuid().ToString().ToLower().Substring(0, 3) : "";
            var availableSpaceForStudyAndSandboxName = maxLength - prefixLength - suffixLength - shortUniquePart.Length - (avoidDash ? 0 : 1);

            var alphanumericStudyName = RemoveSpecialCharactersAndRemoveWhitespace(studyName, avoidDash);
            var alphanumericSandboxName = sandboxName != null ? RemoveSpecialCharactersAndRemoveWhitespace(sandboxName, avoidDash) : null;

            StripTextsEqually(availableSpaceForStudyAndSandboxName, ref alphanumericStudyName, ref alphanumericSandboxName);

            return String.IsNullOrWhiteSpace(alphanumericSandboxName) ?
                $"{prefix}{alphanumericStudyName}{suffix}{shortUniquePart}" :
                $"{prefix}{alphanumericStudyName}{(avoidDash ? "" : "-")}{alphanumericSandboxName}{suffix}{shortUniquePart}"
                ;
        }

        static void StripTextsEqually(int availableSpaceForStudyAndSandboxName, ref string text1, ref string text2)
        {
            var charachtersLeft = availableSpaceForStudyAndSandboxName - (text1.Length + GetLengthOfPotentiallyEmptyText(text2));

            //Strip some characters off the names
            if (charachtersLeft < 0)
            {
                var totalTrim = Math.Abs(charachtersLeft);

                var nameLengthDiff = text1.Length - GetLengthOfPotentiallyEmptyText(text2);
                var amountToTrimOff = Math.Abs(nameLengthDiff) > totalTrim ? totalTrim : Math.Abs(nameLengthDiff);

                if (nameLengthDiff > 0) // study name is longer, trim it down to sandbox length
                {
                    text1 = text1.Substring(0, text1.Length - amountToTrimOff);
                }
                else // sandbox name is longer, trim it down to study length
                {
                    text2 = text2.Substring(0, GetLengthOfPotentiallyEmptyText(text2) - amountToTrimOff);
                }

                //Both names are now equal in length, now we can equally remove from both

                charachtersLeft = availableSpaceForStudyAndSandboxName - (text1.Length + GetLengthOfPotentiallyEmptyText(text2));

                if (charachtersLeft < 0)
                {
                    var mustRemoveEach = Math.Abs(charachtersLeft) / 2;
                    var even = charachtersLeft % 2 == 0;
                    text1 = EnsureMaxLength(text1, text1.Length - mustRemoveEach - (even ? 0 : 1));
                    text2 = EnsureMaxLength(text2, GetLengthOfPotentiallyEmptyText(text2) - mustRemoveEach);
                }
            }
        }

        static int GetLengthOfPotentiallyEmptyText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            return text.Length;
        }

        static string StripWhitespaceAndEnsureLength(string input, int limit = 0)
        {
            var normalizedString = StripWhitespace(input).ToLower();

            return EnsureMaxLength(normalizedString, limit);
        }

        public static string RemoveSpecialCharactersAndRemoveWhitespace(string str, bool avoidDash = false, int limit = 0)
        {

            var alphaNummericString = new string((from c in str
                                                  where (char.IsLetterOrDigit(c) || (c.Equals('-') && !avoidDash)) && !char.IsWhiteSpace(c) && c != 'æ' && c != 'ø' && c != 'å'
                                                  select c).ToArray()).ToLower();

            return EnsureMaxLength(alphaNummericString, limit);
        }

        public static string StripWhitespace(string str) => new string((from c in str
                                                                        where !char.IsWhiteSpace(c) && c != 'æ' && c != 'ø' && c != 'å'
                                                                        select c
                                                                ).ToArray());


    }
}
