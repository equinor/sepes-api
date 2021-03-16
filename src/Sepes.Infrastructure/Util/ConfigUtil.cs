using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Util
{
    public static class ConfigUtil
    {
        public static HashSet<string> GetCommaSeparatedConfigValueAndThrowIfEmpty(IConfiguration config, string configKey)
        {
            var valueRaw = GetConfigValueAndThrowIfEmpty(config, configKey);

            if (valueRaw.Contains(","))
            {
                var result = new HashSet<string>();

                foreach(var curValuePart in valueRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
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
