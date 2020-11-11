using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineRuleService : IVirtualMachineRuleService
    {
        readonly ILogger _logger;
        readonly SepesDbContext _db;
        readonly IUserService _userService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IProvisioningQueueService _workQueue;

        public VirtualMachineRuleService(ILogger<VirtualMachineService> logger,
            SepesDbContext db,
            IUserService userService,
            ISandboxResourceService sandboxResourceService,
            ISandboxResourceOperationService sandboxResourceOperationService,
            IProvisioningQueueService workQueue)
        {
            _logger = logger;
            _db = db;
            _userService = userService;
            _sandboxResourceService = sandboxResourceService;
            _sandboxResourceOperationService = sandboxResourceOperationService;
            _workQueue = workQueue;
        }      

        async Task<SandboxResource> GetVmResourceEntry(int vmId, UserOperations operation)
        {
            _ = await StudyAccessUtil.GetStudyByResourceIdCheckAccessOrThrow(_db, _userService, vmId, operation);
            var vmResource = await _sandboxResourceService.GetByIdAsync(vmId);

            return vmResource;
        }

        public async Task<VmRuleDto> AddRule(int vmId, VmRuleDto input, CancellationToken cancellationToken = default)
        {
            await ValidateRuleThrowIfInvalid(vmId, input);

            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            ThrowIfRuleExists(vmSettings, input);

            input.Id = Guid.NewGuid().ToString();

            if (vmSettings.Rules == null)
            {
                vmSettings.Rules = new List<VmRuleDto>();
            }

            vmSettings.Rules.Add(input);

            vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

            await _db.SaveChangesAsync();

            await CreateUpdateOperationAndAddQueueItem(vm, "Add rule");

            return input;
        }

        public async Task<VmRuleDto> GetRuleById(int vmId, string ruleId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {
                foreach (var curExistingRule in vmSettings.Rules)
                {
                    if (curExistingRule.Id == ruleId)
                    {
                        return curExistingRule;
                    }
                }
            }

            throw new NotFoundException($"Rule with id {ruleId} does not exist");
        }

        public async Task<List<VmRuleDto>> SetRules(int vmId, List<VmRuleDto> updatedRuleSet, CancellationToken cancellationToken = default)
        {

            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            bool saveAfterwards = false;

            if (updatedRuleSet == null || updatedRuleSet != null && updatedRuleSet.Count == 0) //Easy, all rules should be deleted
            {
                vmSettings.Rules = null;
                saveAfterwards = true;
            }
            else
            {
                foreach(var curRule in updatedRuleSet)
                {
                    await ValidateRuleThrowIfInvalid(vmId, curRule);
                }

                var newRules = updatedRuleSet.Where(r => String.IsNullOrWhiteSpace(r.Id)).ToList();
                var existingRules = updatedRuleSet.Where(r => !String.IsNullOrWhiteSpace(r.Id)).ToList();

                foreach(var curNew in newRules)
                {
                    ThrowIfRuleExists(existingRules, curNew);
                }

                foreach (var curRule in updatedRuleSet)
                {
                    if (String.IsNullOrWhiteSpace(curRule.Id))
                    {
                        curRule.Id = Guid.NewGuid().ToString();
                        curRule.Priority = AzureVmUtil.GetNextVmRulePriority(updatedRuleSet, curRule.Direction);
                    }
                }

                vmSettings.Rules = updatedRuleSet;
                saveAfterwards = true;
            }  
            
            if (saveAfterwards)
            {
                vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

                await _db.SaveChangesAsync();

                await CreateUpdateOperationAndAddQueueItem(vm, "Updated rules");
            }

            return updatedRuleSet != null? updatedRuleSet : new List<VmRuleDto>();
        }

        public async Task<VmRuleDto> UpdateRule(int vmId, VmRuleDto input, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {

                VmRuleDto ruleToRemove = null;

                var rulesDictionary = vmSettings.Rules.ToDictionary(r => r.Id, r => r);

                if (rulesDictionary.TryGetValue(input.Id, out ruleToRemove))
                {
                    vmSettings.Rules.Remove(ruleToRemove);

                    ThrowIfRuleExists(vmSettings, input);

                    vmSettings.Rules.Add(input);

                    vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

                    await _db.SaveChangesAsync();

                    await CreateUpdateOperationAndAddQueueItem(vm, "Update rule");

                    return input;
                }
            }

            throw new NotFoundException($"Rule with id {input.Id} does not exist");
        }

        public async Task<List<VmRuleDto>> GetRules(int vmId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            return vmSettings.Rules != null ? vmSettings.Rules : new List<VmRuleDto>();
        }

        public async Task<VmRuleDto> DeleteRule(int vmId, string ruleId, CancellationToken cancellationToken = default)
        {
            var vm = await GetVmResourceEntry(vmId, UserOperations.SandboxEdit);

            //Get config string
            var vmSettings = SandboxResourceConfigStringSerializer.VmSettings(vm.ConfigString);

            if (vmSettings.Rules != null)
            {

                VmRuleDto ruleToRemove = null;

                var rulesDictionary = vmSettings.Rules.ToDictionary(r => r.Id, r => r);

                if (rulesDictionary.TryGetValue(ruleId, out ruleToRemove))
                {
                    vmSettings.Rules.Remove(ruleToRemove);

                    vm.ConfigString = SandboxResourceConfigStringSerializer.Serialize(vmSettings);

                    await _db.SaveChangesAsync();

                    await CreateUpdateOperationAndAddQueueItem(vm, "Delete rule");

                    return ruleToRemove;
                }
            }

            throw new NotFoundException($"Rule with id {ruleId} does not exist");
        }

        async Task ValidateRuleThrowIfInvalid(int vmId, VmRuleDto input)
        {
            if (true)
            {
                return;
            }

            throw new Exception($"Cannot apply rule to VM {vmId}");
        }


        void ThrowIfRuleExists(VmSettingsDto vmSettings, VmRuleDto ruleToCompare)
        {
            ThrowIfRuleExists(vmSettings.Rules, ruleToCompare);
        }

        void ThrowIfRuleExists(List<VmRuleDto> rules, VmRuleDto ruleToCompare)
        {
            if (rules != null)
            {
                foreach (var curExistingRule in rules)
                {
                    if (AzureVmUtil.IsSameRule(ruleToCompare, curExistingRule))
                    {
                        throw new Exception($"Same rule allready exists");
                    }
                }
            }
        }

        async Task<SandboxResourceOperationDto> CreateUpdateOperationAndAddQueueItem(SandboxResource vm, string description)
        {
            var vmUpdateOperation = await _sandboxResourceOperationService.CreateUpdateOperationAsync(vm.Id);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = vm.SandboxId;
            queueParentItem.Description = $"Update VM state for Sandbox: {vm.SandboxId} ({description})";

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = vmUpdateOperation.Id.Value });

            await _workQueue.SendMessageAsync(queueParentItem);

            return vmUpdateOperation;
        }
    }
}
