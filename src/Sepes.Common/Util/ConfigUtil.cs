using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Sepes.Common.Util
{
    public static class ConfigUtil
    {
        public static HashSet<string> GetCommaSeparatedConfigValueAndThrowIfEmpty(IConfiguration config, string configKey)
        {
            var valueRaw = GetConfigValueAndThrowIfEmpty(config, configKey);

            if (valueRaw.Contains(","))
            {
                var result = new HashSet<string>();

                foreach (var curValuePart in valueRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!String.IsNullOrWhiteSpace(curValuePart))
                    {
                        result.Add(curValuePart.Trim());
                    }
                }

                return result;
            }
            else
            {
                return new HashSet<string>() { valueRaw };
            }
        }

        public static string GetConfigValueAndThrowIfEmpty(IConfiguration config, string configKey)
        {
            if (String.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentNullException("configKey", "The parameter configKey was null or empty");
            }

            var configValue = config[configKey];

            if (String.IsNullOrWhiteSpace(configValue))
            {
                throw new Exception($"The configuration entry for {configKey} was empty");
            }

            return configValue;
        }

        public static string RemovePasswordFromConnectionString(string connectionString)
        {
            var result = RemovePasswordWithKey(connectionString, "pwd");
            result = RemovePasswordWithKey(result, "password");
            return result;
        }

        public static string RemovePasswordWithKey(string sourceString, string pwdKeyName)
        {
            var replaceText = "<password removed>";

            var indexOfPwdPart = sourceString.IndexOf(pwdKeyName+"=", comparisonType: StringComparison.InvariantCultureIgnoreCase);
            var lengthOfPwdPart = pwdKeyName.Length + 1;

            if (indexOfPwdPart < 0)
            {
                return sourceString;
            }

            var indexOfEndSectionSemicolon = sourceString.IndexOf(";", indexOfPwdPart +1, comparisonType: StringComparison.InvariantCultureIgnoreCase);

            if (indexOfEndSectionSemicolon < 0)
            {
                throw new Exception("Unable to determine end of password section");
            }

            var passwordStartIndex = indexOfPwdPart + lengthOfPwdPart;          

            return $"{sourceString.Substring(0, passwordStartIndex)}{replaceText}{sourceString.Substring(indexOfEndSectionSemicolon)}";
        }

        public static bool GetBoolConfig(IConfiguration config, string configKey)
        {
            if (String.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentNullException("configKey", "The parameter configKey was null or empty");
            }

            var configValue = config[configKey];

            if (String.IsNullOrWhiteSpace(configValue))
            {
                return false;
            }

            return configValue.ToLower().Equals("true");
        }
    }
}
