using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Common.Exceptions;
using Sepes.Common.Model;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineRuleService : VirtualMachineServiceBase, IVirtualMachineRuleService
    {

        readonly ICloudResourceOperationReadService _sandboxResourceOperationReadService;
        readonly ICloudResourceOperationCreateService _sandboxResourceOperationCreateService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public VirtualMachineRuleService(
            IConfiguration configuration,
            SepesDbContext db,
            ILogger<VirtualMachineRuleService> logger,
            IMapper mapper,
            IUserService userService,
            ICloudResourceReadService cloudResourceReadService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationReadService sandboxResourceOperationReadService,
            ICloudResourceOperationCreateService sandboxResourceOperationCreateService)
             : base(configuration, db, logger, mapper, userService, cloudResourceReadService)
        {
            _provisioningQueueService = provisioningQueueService;
            _sandboxResourceOperationReadService = sandboxResourceOperationReadService;
            _sandboxResourceOperationCreateService = sandboxResourceOperationCreateService;

        }

        public async Task<VmRuleDto> GetRuleById(int vmId, string ruleId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVirtualMachineResourceEntry(vmId, UserOperation.Study_Read);

            //Get config string
            var vmSettings = CloudResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {
                return vmSettings.Rules.SingleOrDefault(r => r.Name == ruleId);
            }

            throw new NotFoundException($"Rule with id {ruleId} does not exist");
        }

        public async Task<List<VmRuleDto>> SetRules(int vmId, List<VmRuleDto> updatedRuleSet, CancellationToken cancellationToken = default)
        {
            var vm = await GetVirtualMachineResourceEntry(vmId, UserOperation.Study_Crud_Sandbox);

            //Get config string
            var vmSettings = CloudResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            await ValidateRuleUpdateInputThrowIfNot(vm, vmSettings.Rules, updatedRuleSet);

            bool saveAfterwards = false;

            if (updatedRuleSet == null || updatedRuleSet != null && updatedRuleSet.Count == 0) //Easy, all rules should be deleted
            {
                vmSettings.Rules = null;
                saveAfterwards = true;
            }
            else
            {
                var newRules = updatedRuleSet.Where(r => String.IsNullOrWhiteSpace(r.Name)).ToList();
                var rulesThatShouldExistAllready = updatedRuleSet.Where(r => !String.IsNullOrWhiteSpace(r.Name)).ToList();

                //Check that the new rules does not have a duplicate in existing rules
                foreach (var curNew in newRules)
                {
                    ThrowIfRuleExists(rulesThatShouldExistAllready, curNew);
                }

                foreach (var curRule in updatedRuleSet)
                {
                    if (curRule.Direction == RuleDirection.Inbound)
                    {
                        if (curRule.Action == RuleAction.Deny)
                        {
                            throw new ArgumentException("Inbound rules can only have Action: Allow");
                        }

                        if (String.IsNullOrWhiteSpace(curRule.Name))
                        {
                            curRule.Name = AzureResourceNameUtil.NsgRuleNameForVm(vmId);
                            //curRule.Priority = AzureVmUtil.GetNextVmRulePriority(updatedRuleSet, curRule.Direction);
                        }
                    }
                    else
                    {
                        if (String.IsNullOrWhiteSpace(curRule.Name) || !curRule.Name.Contains(AzureVmConstants.RulePresets.OPEN_CLOSE_INTERNET))
                        {
                            throw new ArgumentException("Custom outbound rules are not allowed");
                        }
                    }
                }

                vmSettings.Rules = updatedRuleSet;
                saveAfterwards = true;
            }

            if (saveAfterwards)
            {
                vm.ConfigString = CloudResourceConfigStringSerializer.Serialize(vmSettings);

                await _db.SaveChangesAsync();

                await CreateUpdateOperationAndAddQueueItem(vm, "Updated rules");
            }

            return updatedRuleSet != null ? updatedRuleSet : new List<VmRuleDto>();
        }

        async Task ValidateRuleUpdateInputThrowIfNot(CloudResource vm, List<VmRuleDto> existingRules, List<VmRuleDto> updatedRuleSet)
        {
            var validationErrors = new List<string>();

            var sandbox = await _db.Sandboxes.Include(sb => sb.PhaseHistory).FirstOrDefaultAsync(sb => sb.Id == vm.SandboxId);
            var curPhase = SandboxPhaseUtil.GetCurrentPhase(sandbox);

            //VALIDATE OUTBOUND RULE, THERE SHOULD BE ONLY ONE

            var outboundRules = updatedRuleSet.Where(r => r.Direction == RuleDirection.Outbound).ToList();

            if (outboundRules.Count != 1)
            {
                validationErrors.Add($"Multiple outbound rule(s) provided");
                ValidationUtils.ThrowIfValidationErrors("Rule update not allowed", validationErrors);
            }

            var onlyOutboundRuleFromExisting = existingRules.SingleOrDefault(r => r.Direction == RuleDirection.Outbound);
            var onlyOutboundRuleFromClient = outboundRules.SingleOrDefault();

            if (onlyOutboundRuleFromExisting.Name != onlyOutboundRuleFromClient.Name)
            {
                validationErrors.Add($"Illegal outbound rule(s) provided");
                ValidationUtils.ThrowIfValidationErrors("Rule update not allowed", validationErrors);
            }

            //If Sandbox is not open, make sure outbound rule has not changed
            if (curPhase > SandboxPhase.Open
                && onlyOutboundRuleFromClient.Direction == RuleDirection.Outbound
                && onlyOutboundRuleFromClient.ToString() != onlyOutboundRuleFromExisting.ToString())
            {
                var currentUser = await _userService.GetCurrentUserAsync();

                if (!currentUser.Admin)
                {
                    validationErrors.Add($"Only admin can updated outgoing rules when Sandbox is in phase {curPhase}");
                    ValidationUtils.ThrowIfValidationErrors("Rule update not allowed", validationErrors);
                }
            }

            //VALIDATE INBOUND RULES

            foreach (var curInboundRule in updatedRuleSet.Where(r => r.Direction == RuleDirection.Inbound).ToList())
            {
                if (curInboundRule.Direction > RuleDirection.Outbound)
                {
                    validationErrors.Add($"Invalid direction for rule {curInboundRule.Description}: {curInboundRule.Direction}");
                }

                if (String.IsNullOrWhiteSpace(curInboundRule.Ip))
                {
                    validationErrors.Add($"Missing ip for rule {curInboundRule.Description}");
                }

                if (curInboundRule.Port <= 0)
                {
                    validationErrors.Add($"Invalid port for rule {curInboundRule.Description}: {curInboundRule.Port}");
                }

                if (String.IsNullOrWhiteSpace(curInboundRule.Description))
                {
                    validationErrors.Add($"Missing Description for rule {curInboundRule.Description}");
                }
            }

            ValidationUtils.ThrowIfValidationErrors("Rule update not allowed", validationErrors);
        }

        public async Task<bool> IsInternetVmRuleSetToDeny(int vmId)
        {
            var internetRule = await GetInternetRule(vmId);

            if (internetRule == null)
            {
                throw new NotFoundException($"Could not find internet rule for VM {vmId}");
            }

            return IsRuleSetToDeny(internetRule);
        }

        public bool IsRuleSetToDeny(VmRuleDto rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }

            return rule.Action == RuleAction.Deny;
        }

        public async Task<VmRuleDto> GetInternetRule(int vmId)
        {
            var vm = await GetVirtualMachineResourceEntry(vmId, UserOperation.Study_Crud_Sandbox);

            //Get config string
            var vmSettings = CloudResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {
                return vmSettings.Rules.SingleOrDefault(r => r.Direction == RuleDirection.Outbound
                && r.Name.Contains(AzureVmConstants.RulePresets.OPEN_CLOSE_INTERNET));
            }

            return null;
        }

        public async Task<List<VmRuleDto>> GetRules(int vmId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVirtualMachineResourceEntry(vmId, UserOperation.Study_Read);

            //Get config string
            var vmSettings = CloudResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            return vmSettings.Rules != null ? vmSettings.Rules : new List<VmRuleDto>();
        }

        void ThrowIfRuleExists(List<VmRuleDto> rules, VmRuleDto ruleToCompare)
        {
            if (rules != null
                && rules.Any(r => AzureVmUtil.IsSameRule(ruleToCompare, r)))
            {
                throw new Exception($"Same rule allready exists");
            }
        }

        async Task CreateUpdateOperationAndAddQueueItem(CloudResource vm, string description)
        {
            //If un-started update allready exist, no need to create update op?
            if (await _sandboxResourceOperationReadService.HasUnstartedCreateOrUpdateOperation(vm.Id))
            {
                _logger.LogWarning($"Updating VM {vm.Id}: There is allready an unstarted VM Create or Update operation. Not creating additional");
            }
            else
            {
                var vmUpdateOperation = await _sandboxResourceOperationCreateService.CreateUpdateOperationAsync(vm.Id);

                await _provisioningQueueService.CreateItemAndEnqueue(vmUpdateOperation);
            }
        }
    }
}
