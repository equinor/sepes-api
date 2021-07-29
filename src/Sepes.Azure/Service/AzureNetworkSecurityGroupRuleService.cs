using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Azure.Service.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto;

namespace Sepes.Azure.Service
{
    public class AzureNetworkSecurityGroupRuleService : AzureSdkServiceBase, IAzureNetworkSecurityGroupRuleService
    {
        public AzureNetworkSecurityGroupRuleService(IConfiguration config, ILogger<AzureNetworkSecurityGroupService> logger)
             : base(config, logger)
        {


        }

        public async Task<Dictionary<string, NsgRuleDto>> GetNsgRulesContainingName(string resourceGroupName, string nsgName, string nameContains, CancellationToken cancellationToken = default)
        {
            var nsg = await _azure.NetworkSecurityGroups.GetByResourceGroupAsync(resourceGroupName, nsgName, cancellationToken);

            var result = new Dictionary<string, NsgRuleDto>();

            foreach (var curRuleKvp in nsg.SecurityRules)
            {
                if (curRuleKvp.Value.Name.Contains(nameContains))
                {
                    if (!result.ContainsKey(curRuleKvp.Value.Name))
                    {
                        result.Add(curRuleKvp.Value.Name, new NsgRuleDto() { Key = curRuleKvp.Key, Name = curRuleKvp.Value.Name, Description = curRuleKvp.Value.Description, Protocol = curRuleKvp.Value.Protocol, Priority = curRuleKvp.Value.Priority, Direction = curRuleKvp.Value.Direction });
                    }
                }
            }

            return result;
        }

        public async Task<bool> IsRuleSetTo(string resourceGroupName, string nsgName, string ruleName, RuleAction action, CancellationToken cancellationToken = default)
        {
            var nsg = await _azure.NetworkSecurityGroups.GetByResourceGroupAsync(resourceGroupName, nsgName, cancellationToken);

            foreach (var curRuleKvp in nsg.SecurityRules)
            {
                if (curRuleKvp.Value.Name == ruleName)
                {
                    //TODO: VERIFY CHECK
                    if (curRuleKvp.Value.Access.ToLower() == action.ToString().ToLower())
                    {
                        return true;
                    }
                }
            }


            return false;
        }


        public async Task<Dictionary<string, NsgRuleDto>> GetNsgRulesForDirection(string resourceGroupName, string nsgName, string direction, CancellationToken cancellationToken = default)
        {
            var nsg = await _azure.NetworkSecurityGroups.GetByResourceGroupAsync(resourceGroupName, nsgName, cancellationToken);

            var result = new Dictionary<string, NsgRuleDto>();

            foreach (var curRuleKvp in nsg.SecurityRules)
            {
                if (curRuleKvp.Value.Direction == direction)
                {
                    if (!result.ContainsKey(curRuleKvp.Value.Name))
                    {
                        result.Add(curRuleKvp.Value.Name, new NsgRuleDto() { Key = curRuleKvp.Key, Name = curRuleKvp.Value.Name, Description = curRuleKvp.Value.Description, Protocol = curRuleKvp.Value.Protocol, Priority = curRuleKvp.Value.Priority, Direction = curRuleKvp.Value.Direction });
                    }
                }
            }

            return result;
        }

        public async Task AddInboundRule(string resourceGroupName, string securityGroupName,
                                      NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var createOperation = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update()
                 .DefineRule(rule.Name);

            var operationWithRules = (rule.Action == RuleAction.Allow ? createOperation.AllowInbound() : createOperation.DenyInbound())
            .FromAddresses(rule.SourceAddress)
            .FromAnyPort()
            .ToAddresses(rule.DestinationAddress);

            _ = await (rule.DestinationPort == 0 ? operationWithRules.ToAnyPort() : operationWithRules.ToPort(rule.DestinationPort))
            .WithAnyProtocol()
            .WithPriority(rule.Priority)
            .WithDescription(rule.Description)
            .Attach()
            .ApplyAsync(cancellationToken);
        }

        public async Task UpdateInboundRule(string resourceGroupName, string securityGroupName,
                                 NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var updateNsgOperation = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update();


            var updateRuleOp = updateNsgOperation
             .UpdateRule(rule.Name);
            //Decide of allow or deny
            (rule.Action == RuleAction.Allow ? updateRuleOp.AllowInbound() : updateRuleOp.DenyInbound())

                .FromAddresses(rule.SourceAddress)
            .FromAnyPort()
              .ToAddresses(rule.DestinationAddress);
            _ = (rule.DestinationPort == 0 ? updateRuleOp.ToAnyPort() : updateRuleOp.ToPort(rule.DestinationPort))

           .WithAnyProtocol()
           .WithPriority(rule.Priority)
             .WithDescription(rule.Description);
            await updateNsgOperation.ApplyAsync();
        }

        public async Task AddOutboundRule(string resourceGroupName, string securityGroupName,
                                    NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var operationStep1 = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update()
                 .DefineRule(rule.Name);

            var operationStep2 = (rule.Action == RuleAction.Allow ? operationStep1.AllowOutbound() : operationStep1.DenyOutbound())
                 .FromAddresses(rule.SourceAddress);

            var operationStep3 = (rule.SourcePort == 0 ? operationStep2.FromAnyPort() : operationStep2.FromPort(rule.SourcePort));

            var operationStep4 = (rule.DestinationAddress == "*" ? operationStep3.ToAnyAddress() : operationStep3.ToAddress(rule.DestinationAddress));

            var operationStep5 = operationStep4
                .ToAnyPort()
                .WithAnyProtocol()
                .WithPriority(rule.Priority)
                    .WithDescription(rule.Description)
                .Attach();

            await operationStep5
              .ApplyAsync(cancellationToken);
        }


        public async Task UpdateOutboundRule(string resourceGroupName, string securityGroupName,
                                   NsgRuleDto rule, CancellationToken cancellationToken = default)
        {
            var operationStep1 = _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update();

            var operationStep2 = operationStep1
             .UpdateRule(rule.Name);

            var operationStep3 = (rule.Action == RuleAction.Allow ? operationStep2.AllowOutbound() : operationStep2.DenyOutbound())
              .FromAddresses(rule.SourceAddress);

            var operationStep4 = (rule.SourcePort == 0 ? operationStep3.FromAnyPort() : operationStep3.FromPort(rule.SourcePort));
            //ruleMapped.DestinationAddress = "*";
            //ruleMapped.DestinationPort = 0;

            var operationStep5 = (rule.DestinationAddress == "*" ? operationStep4.ToAnyAddress() : operationStep2.ToAddress(rule.DestinationAddress));

            _ = operationStep5
                  .ToAnyPort()
                  .WithAnyProtocol()
                  .WithPriority(rule.Priority)
                      .WithDescription(rule.Description);

            await operationStep1.ApplyAsync();
        }

        public async Task DeleteRule(string resourceGroupName, string securityGroupName,
                                string ruleName, CancellationToken cancellationToken = default)
        {
            await _azure.NetworkSecurityGroups
                 .GetByResourceGroup(resourceGroupName, securityGroupName)
                 .Update()
                 .WithoutRule(ruleName)
                .ApplyAsync();

        }

    }
}
