using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public static async Task EnsureDatasetHasFirewallRules(IConfiguration config, ILogger logger, UserDto user, Dataset dataset, string clientIp)
        {
            var serverPublicIp = await IpAddressUtil.GetServerPublicIp(config);

            SetDatasetFirewallRules(user, dataset, clientIp, serverPublicIp);

            logger.LogInformation($"Creating firewall rules for dataset {dataset.Id}, clientIp: {clientIp}, serverIp: {serverPublicIp}");
        }

        public static bool SetDatasetFirewallRules(UserDto user, Dataset dataset, string clientIp, string serverIp)
        {
            var clientRule = DatasetFirewallUtils.CreateClientRule(user, clientIp);
            var serverRule = DatasetFirewallUtils.CreateServerRule(user, serverIp);

            bool anyChanges = false;

            if (dataset.FirewallRules == null)
            {
                dataset.FirewallRules = new List<DatasetFirewallRule>();
            }

            var newRuleSet = RemoveOldRules(dataset.FirewallRules);

            if (newRuleSet.Count < dataset.FirewallRules.Count)
            {
                anyChanges = true;
            }

            if (ClientIpIsValid(clientIp) && !newRuleSet.Where(r => r.RuleType == clientRule.RuleType && r.Address == clientRule.Address).Any())
            {
                newRuleSet.Add(clientRule);
                anyChanges = true;
            }

            if ((String.IsNullOrWhiteSpace(clientIp) || !clientIp.Equals(serverIp)) && !newRuleSet.Where(r => r.RuleType == serverRule.RuleType && r.Address == serverRule.Address).Any())
            {
                newRuleSet.Add(serverRule);
                anyChanges = true;
            }

            if (anyChanges)
            {
                dataset.FirewallRules = newRuleSet;
            }

            return anyChanges;
        }

        static ICollection<DatasetFirewallRule> RemoveOldRules(ICollection<DatasetFirewallRule> source)
        {
            return source.Where(r => r.Created.AddMonths(1) >= DateTime.UtcNow).ToList();
        }

        static bool ClientIpIsValid(string clientIp)
        {
            if (String.IsNullOrWhiteSpace(clientIp))
            {
                return false;
            }

            if (clientIp != "::1" && clientIp != "0.0.0.1")
            {
                return true;
            }

            return false;
        }

        public static DatasetFirewallRule CreateClientRule(UserDto user, string clientIp)
        {
            return CreateRule(user, DatasetFirewallRuleType.Client, clientIp);
        }

        public async static Task<DatasetFirewallRule> CreateServerRuleAsync(IConfiguration config, UserDto user)
        {
            var serverPublicIp = await IpAddressUtil.GetServerPublicIp(config);
            return CreateServerRule(user, serverPublicIp);
        }

        public static DatasetFirewallRule CreateServerRule(UserDto user, string ip)
        {
            return CreateRule(user, DatasetFirewallRuleType.Api, ip);
        }

        static DatasetFirewallRule CreateRule(UserDto user, DatasetFirewallRuleType ruleType, string ipAddress)
        {
            return new DatasetFirewallRule() { CreatedBy = user.UserName, Created = DateTime.UtcNow, RuleType = ruleType, Address = ipAddress };
        }
    }
}
