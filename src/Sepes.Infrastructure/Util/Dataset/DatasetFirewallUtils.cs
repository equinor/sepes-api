using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

        public static void EnsureDatasetHasFirewallRules(ILogger logger, UserDto user, Dataset dataset, string clientIp, string serverPublicIp)
        {
            SetDatasetFirewallRules(user, dataset, clientIp, serverPublicIp);

            logger.LogInformation($"Creating firewall rules for dataset {dataset.Id}, clientIp: {clientIp}, serverIp: {serverPublicIp}");
        }

        public static bool SetDatasetFirewallRules(UserDto user, Dataset dataset, string clientIp, string serverIp)
        {
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

            if (ClientIpIsValid(clientIp))
            {
                var clientRule = DatasetFirewallUtils.CreateClientRule(user, clientIp);

                if (!newRuleSet.Where(r => r.RuleType == clientRule.RuleType && r.Address == clientRule.Address).Any())
                {
                    newRuleSet.Add(clientRule);
                    anyChanges = true;
                }
            }
           

            if (ServerIpIsValid(serverIp))
            {
                var serverRule = DatasetFirewallUtils.CreateServerRule(user, serverIp);

                if (serverRule != null && (String.IsNullOrWhiteSpace(clientIp) || !clientIp.Equals(serverIp)) && !newRuleSet.Where(r => r.RuleType == serverRule.RuleType && r.Address == serverRule.Address).Any())
                {
                    newRuleSet.Add(serverRule);
                    anyChanges = true;
                }
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

        public static bool IpIsValid(string ipAddress, string errorMessage)
        {
            if (String.IsNullOrWhiteSpace(ipAddress))
            {
                return false;
            }

            if (ipAddress != "::1" && ipAddress != "0.0.0.1")
            {
                if (IPAddress.TryParse(ipAddress, out _))
                {
                    return true;
                }

                throw new ArgumentException(errorMessage);
            }

            return false;
        }

        public static bool ClientIpIsValid(string clientIp)
        {
            //Should be blank, or if not blank, it should be valid
            return IpIsValid(clientIp, "Client IP is not a valid IP Address");         
        }

        public static bool ServerIpIsValid(string clientIp)
        {
            //Should be blank, or if not blank, it should be valid
            return IpIsValid(clientIp, "Server IP is not a valid IP Address");
        }

        public static DatasetFirewallRule CreateClientRule(UserDto user, string clientIp)
        {
            return CreateRule(user, DatasetFirewallRuleType.Client, clientIp);
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
