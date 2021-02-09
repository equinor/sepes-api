using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class DatasetFirewallUtils
    {

        public static List<FirewallRule> TranslateAllowedIpsToGenericFirewallRule(List<DatasetFirewallRule> datasetFirewallRules)
        {
            return datasetFirewallRules.Select(r => new FirewallRule() { Action = FirewallRuleAction.Allow, Address = r.Address }).ToList();
        }

        public static string TranslateAllowedIpsToOperationDesiredState(List<DatasetFirewallRule> datasetFirewallRules)
        {
            if (datasetFirewallRules.Count == 0)
            {
                return null;
            }

            var translated = TranslateAllowedIpsToGenericFirewallRule(datasetFirewallRules);

            return CloudResourceConfigStringSerializer.Serialize(translated);
        }

        public static async Task SetDatasetFirewallRules(IConfiguration config, UserDto user, Dataset dataset, string clientIp)
        {
            //add state
            dataset.FirewallRules = new List<DatasetFirewallRule>();

            //Add user's client IP

            if (clientIp != "::1")
            {
                dataset.FirewallRules.Add(CreateClientRule(user, clientIp));
            }

            //Add application IP, so that it can upload/download files 

            dataset.FirewallRules.Add(await CreateServerRuleAsync(config, user));
        }

        static DatasetFirewallRule CreateClientRule(UserDto user, string clientIp)
        {
            return CreateRule(user, DatasetFirewallRuleType.Client, clientIp);
        }

        public async static Task<DatasetFirewallRule> CreateServerRuleAsync(IConfiguration config, UserDto user)
        {
            var serverPublicIp = await IpAddressUtil.GetServerPublicIp(config);
            return CreateRule(user, DatasetFirewallRuleType.Api, serverPublicIp);
        }

        static DatasetFirewallRule CreateRule(UserDto user, DatasetFirewallRuleType ruleType, string ipAddress)
        {
            return new DatasetFirewallRule() { CreatedBy = user.UserName, Created = DateTime.UtcNow, RuleType = ruleType, Address = ipAddress };
        }
    }
}
