using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Util
{
    public static class DatasetCorsUtils
    {

        public static string CreateDatasetCorsRules(IConfiguration config)
        {
            var corsRulesList = new List<CorsRule>();

            var corsDomainsFromConfig = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(config, ConfigConstants.ALLOW_CORS_DOMAINS);

            foreach (var curCorsDomain in corsDomainsFromConfig)
            {
                if (!String.IsNullOrWhiteSpace(curCorsDomain))
                {
                    corsRulesList.Add(new CorsRule() { Address = curCorsDomain });
                }
            }

            return CloudResourceConfigStringSerializer.Serialize(corsRulesList);
        }
    }
}
