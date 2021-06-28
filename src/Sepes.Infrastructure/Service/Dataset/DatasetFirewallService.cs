using Microsoft.Extensions.Logging;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetFirewallService : IDatasetFirewallService
    {
        readonly ILogger _logger;
        readonly IUserService _userService;
        readonly IPublicIpService _publicIpService;

        public DatasetFirewallService(ILogger logger, IUserService userService, IPublicIpService publicIpService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _publicIpService = publicIpService ?? throw new ArgumentNullException(nameof(publicIpService));
        }

        public async Task EnsureDatasetHasFirewallRules(Dataset dataset, string clientIp)
        {            
            await SetDatasetFirewallRules(dataset, clientIp);         
        }

        public async Task<bool> SetDatasetFirewallRules(Dataset dataset, string clientIp)
        {
            _logger.LogInformation($"SetDatasetFirewallRules - dataset: {dataset.Id} - clientIp: {clientIp}");
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

            var currentUser = await _userService.GetCurrentUserAsync();

            if (ClientIpIsValid(clientIp))
            {
                var clientRule = CreateClientRule(currentUser, clientIp);

                if (!newRuleSet.Where(r => r.RuleType == clientRule.RuleType && r.Address == clientRule.Address).Any())
                {
                    newRuleSet.Add(clientRule);
                    anyChanges = true;
                }
            }

            var serverIp = await _publicIpService.GetIp();

            if (ServerIpIsValid(serverIp))
            {
                var serverRule = CreateServerRule(currentUser, serverIp);

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

        bool ClientIpIsValid(string clientIp)
        {
            //Should be blank, or if not blank, it should be valid
            return IpIsValid(clientIp, "Client IP is not a valid IP Address");
        }

        bool ServerIpIsValid(string clientIp)
        {
            //Should be blank, or if not blank, it should be valid
            return IpIsValid(clientIp, "Server IP is not a valid IP Address");
        }

        DatasetFirewallRule CreateClientRule(UserDto user, string clientIp)
        {
            return CreateRule(user, DatasetFirewallRuleType.Client, clientIp);
        }

        DatasetFirewallRule CreateServerRule(UserDto user, string ip)
        {
            return CreateRule(user, DatasetFirewallRuleType.Api, ip);
        }

        DatasetFirewallRule CreateRule(UserDto user, DatasetFirewallRuleType ruleType, string ipAddress)
        {
            return new DatasetFirewallRule() { CreatedBy = user.UserName, Created = DateTime.UtcNow, RuleType = ruleType, Address = ipAddress };
        }

        ICollection<DatasetFirewallRule> RemoveOldRules(ICollection<DatasetFirewallRule> source)
        {
            return source.Where(r => r.Created.AddMonths(1) >= DateTime.UtcNow).ToList();
        }

        bool IpIsValid(string ipAddress, string errorMessage)
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
    }
}
