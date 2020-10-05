using Microsoft.Extensions.Configuration;
using System;

namespace Sepes.Infrastructure.Util
{
    public static class ConfigUtil
    {
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
    }
}
