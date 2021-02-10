using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxPhaseService : SandboxServiceBase, ISandboxPhaseService
    {
        readonly ICloudResourceReadService _sandboxResourceService;
        readonly ICloudResourceOperationReadService _sandboxResourceOperationService;
        readonly IVirtualMachineRuleService _virtualMachineRuleService;

        readonly IAzureVNetService _azureVNetService;
        readonly IAzureStorageAccountService _azureStorageAccountService;
        readonly IAzureNetworkSecurityGroupRuleService _nsgRuleService;

        public SandboxPhaseService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService, ICloudResourceReadService sandboxResourceService, ICloudResourceOperationReadService sandboxResourceOperationService, IVirtualMachineRuleService virtualMachineRuleService,
            IAzureVNetService azureVNetService, IAzureStorageAccountService azureStorageAccountService, IAzureNetworkSecurityGroupRuleService nsgRuleService)
            : base(config, db, mapper, logger, userService)
        {

            _sandboxResourceService = sandboxResourceService;
            _sandboxResourceOperationService = sandboxResourceOperationService;
            _virtualMachineRuleService = virtualMachineRuleService;

            _azureVNetService = azureVNetService;
            _azureStorageAccountService = azureStorageAccountService;
            _nsgRuleService = nsgRuleService;
        }

        public async Task<SandboxDetails> MoveToNextPhaseAsync(int sandboxId, CancellationToken cancellation = default)
        {
            _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Starting", sandboxId);

            SandboxPhaseHistory newestHistoryItem = null;

            bool dataMightHaveBeenChanged = false;

            try
            {
                var user = await _userService.GetCurrentUserAsync();

                var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Sandbox_IncreasePhase, true);

                var currentPhaseItem = SandboxPhaseUtil.GetCurrentPhaseHistoryItem(sandboxFromDb);

                if (currentPhaseItem == null)
                {
                    InitiatePhaseHistory(sandboxFromDb, user);
                    currentPhaseItem = SandboxPhaseUtil.GetCurrentPhaseHistoryItem(sandboxFromDb);
                }

                var nextPhase = SandboxPhaseUtil.GetNextPhase(sandboxFromDb);

                var resourcesForSandbox = await _sandboxResourceService.GetSandboxResources(sandboxId, cancellation);

                await ValidatePhaseMoveThrowIfNot(sandboxFromDb, resourcesForSandbox, currentPhaseItem.Phase, nextPhase, cancellation);

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Moving from {1} to {2}", sandboxId, currentPhaseItem.Phase, nextPhase);

                newestHistoryItem = new SandboxPhaseHistory() { Counter = currentPhaseItem.Counter + 1, Phase = nextPhase, CreatedBy = user.UserName };
                dataMightHaveBeenChanged = true;
                sandboxFromDb.PhaseHistory.Add(newestHistoryItem);
                await _db.SaveChangesAsync();

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Phase added to db. Proceeding to make data available", sandboxId);

                if (nextPhase == SandboxPhase.DataAvailable)
                {
                    await MakeDatasetsAvailable(sandboxFromDb, resourcesForSandbox, cancellation);
                }

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Done", sandboxId);

                return await GetSandboxDetailsInternalAsync(sandboxId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, SepesEventId.SandboxNextPhase, "Sandbox {0}: Phase shift failed.", sandboxId);

                if (dataMightHaveBeenChanged)
                {
                    _logger.LogWarning(ex, SepesEventId.SandboxNextPhase, "Data might have been changed. Rolling back");
                    await MakeDatasetsUnAvailable(sandboxId);
                    await AttemptRollbackPhase(sandboxId, newestHistoryItem);
                }

                throw;
            }
        }

        public async Task ValidatePhaseMoveThrowIfNot(Sandbox sandbox, List<CloudResourceDto> resourcesForSandbox, SandboxPhase currentPhase, SandboxPhase nextPhase, CancellationToken cancellation = default)
        {
            var validationErrors = new List<string>();

            _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Validation phase move from {1} to {2}", sandbox.Id, currentPhase, nextPhase);

            validationErrors.AddRange(VerifyThatSandboxHasDatasets(sandbox));
            validationErrors.AddRange(VerifyBasicResourcesIsFinishedAsync(resourcesForSandbox));
            validationErrors.AddRange(await VerifyInternetClosed(sandbox, resourcesForSandbox, cancellation));

            ValidationUtils.ThrowIfValidationErrors("Phase change not allowed", validationErrors);
        }

        List<string> VerifyThatSandboxHasDatasets(Sandbox sandbox)
        {
            var validationErrors = new List<string>();

            if (sandbox.SandboxDatasets.Count == 0)
            {
                validationErrors.Add($"Sandbox contains no Datasets");
            }

            return validationErrors;
        }

        List<string> VerifyBasicResourcesIsFinishedAsync(List<CloudResourceDto> resourcesForSandbox)
        {
            var validationErrors = new List<string>();

            foreach (var curResource in resourcesForSandbox)
            {
                foreach (var curOperation in curResource.Operations)
                {
                    if (curOperation.Status == CloudResourceOperationState.IN_PROGRESS)
                    {
                        validationErrors.Add($"One or more resources are beging created, updated or deleted");
                        return validationErrors;
                    }
                }
            }

            return validationErrors;

        }


        async Task<List<string>> VerifyInternetClosed(Sandbox sandbox, List<CloudResourceDto> resourcesForSandbox, CancellationToken cancellation = default)
        {
            var validationErrors = new List<string>();

            _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Verifying that internet is closed for all VMs ", sandbox.Id);

            var allVms = CloudResourceUtil.GetAllResourcesByType(resourcesForSandbox, AzureResourceType.VirtualMachine, false);

            var networkSecurityGroup = CloudResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.NetworkSecurityGroup, true);

            bool anyVmsFound = false;

            foreach (var curVm in allVms)
            {
                anyVmsFound = true;

                var vmInternetRule = await _virtualMachineRuleService.GetInternetRule(curVm.Id);

                //Check if internet is set to open in Sepes
                if (_virtualMachineRuleService.IsRuleSetToDeny(vmInternetRule) == false)
                {
                    validationErrors.Add($"Internet is set to open on VM {curVm.ResourceName}");
                }
                else if (await _nsgRuleService.IsRuleSetTo(curVm.ResourceGroupName, networkSecurityGroup.ResourceName, vmInternetRule.Name, RuleAction.Allow)) //Verify that internet is actually closed in Network Security Group in Azure
                {
                    validationErrors.Add($"Internet is actually open on VM in Azure {curVm.ResourceName}");
                }

                if (await _sandboxResourceOperationService.HasUnstartedCreateOrUpdateOperation(curVm.Id)) //Other unfinished VM update
                {
                    validationErrors.Add($"Unfinished operation exists for VM {curVm.ResourceName}");
                }
            }

            if (anyVmsFound == false)
            {
                validationErrors.Add($"Sandbox contains no Virtual Machines");
            }

            return validationErrors;
        }

        async Task MakeDatasetsAvailable(Sandbox sandbox, List<CloudResourceDto> resourcesForSandbox, CancellationToken cancellation = default)
        {
            var resourceGroupResource = CloudResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.ResourceGroup, true);
            var vNetResource = CloudResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.VirtualNetwork, true);

            if (resourceGroupResource == null)
            {
                throw new Exception($"Could not locate Resource Group entry for Sandbox {sandbox.Id}");
            }

            if (vNetResource == null)
            {
                throw new Exception($"Could not locate VNet entry for Sandbox {sandbox.Id}");
            }

            await _azureVNetService.EnsureSandboxSubnetHasServiceEndpointForStorage(resourceGroupResource.ResourceName, vNetResource.ResourceName);

            foreach (var curDatasetRelation in sandbox.SandboxDatasets)
            {
                if (curDatasetRelation.Dataset.StudyId.HasValue && curDatasetRelation.Dataset.StudyId == sandbox.StudyId)
                {
                    var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(curDatasetRelation.Dataset);
                    await _azureStorageAccountService.AddStorageAccountToVNet(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName, resourceGroupResource.ResourceName, vNetResource.ResourceName, cancellation);
                }
                else
                {
                    throw new Exception($"Only study specific datasets are supported. Please remove dataset {curDatasetRelation.Dataset.Name} from Sandbox");
                }
            }
        }


        async Task AttemptRollbackPhase(int sandboxId, SandboxPhaseHistory phaseToRemove)
        {
            try
            {
                _logger.LogWarning($"Rolling back phase for sandbox {sandboxId}.");

                if (phaseToRemove != null)
                {
                    if (phaseToRemove.Id > 0)
                    {
                        var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Sandbox_IncreasePhase, true);

                        if (sandboxFromDb.PhaseHistory.Count > 0 && sandboxFromDb.PhaseHistory.Contains(phaseToRemove))
                        {
                            _logger.LogWarning($"Rolling back phase rollback for sandbox {sandboxId}. Phase item: {phaseToRemove.Id}, phase: {phaseToRemove.Phase}");
                            sandboxFromDb.PhaseHistory.Remove(phaseToRemove);
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            _logger.LogWarning($"Attempted phase rollback for sandbox {sandboxId} aborted. Phase record not found associated to Sandbox");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Attempted phase rollback for sandbox {sandboxId} aborted. Phase record Id was 0");
                    }
                }
                else
                {
                    _logger.LogWarning($"Attempted phase rollback for sandbox {sandboxId} aborted. Phase record was NULL");
                }


            }
            catch (Exception ex)
            {
                var additionalInfo = phaseToRemove != null ? $"Phase item: {phaseToRemove.Id}, phase: {phaseToRemove.Phase}" : "";

                _logger.LogError(ex, $"Attempted phase rollback for sandbox {sandboxId} failed. {additionalInfo}");
            }
        }

        async Task MakeDatasetsUnAvailable(int sandboxId)
        {
            var sandbox = await GetWithoutChecks(sandboxId);
            var resourcesForSandbox = await _sandboxResourceService.GetSandboxResources(sandboxId);
            await MakeDatasetsUnAvailable(sandbox, resourcesForSandbox, true);
        }

        async Task MakeDatasetsUnAvailable(Sandbox sandbox, List<CloudResourceDto> resourcesForSandbox, bool continueOnError = true, CancellationToken cancellation = default)
        {
            var resourceGroupResource = CloudResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.ResourceGroup, true);
            var vNetResource = CloudResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.VirtualNetwork, true);

            if (resourceGroupResource == null)
            {
                throw new Exception($"Could not locate Resource Group entry for Sandbox {sandbox.Id}");
            }

            if (vNetResource == null)
            {
                throw new Exception($"Could not locate VNet entry for Sandbox {sandbox.Id}");
            }

            foreach (var curDatasetRelation in sandbox.SandboxDatasets)
            {
                try
                {
                    if (curDatasetRelation.Dataset.StudyId.HasValue && curDatasetRelation.Dataset.StudyId == sandbox.StudyId)
                    {
                        var datasetResourceEntry = DatasetUtils.GetStudySpecificStorageAccountResourceEntry(curDatasetRelation.Dataset);
                        await _azureStorageAccountService.RemoveStorageAccountFromVNet(datasetResourceEntry.ResourceGroupName, datasetResourceEntry.ResourceName, resourceGroupResource.ResourceName, vNetResource.ResourceName, cancellation);
                    }
                    else
                    {
                        throw new Exception($"Only study specific datasets are supported. Please remove dataset {curDatasetRelation.Dataset.Name} from Sandbox");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to make dataset {curDatasetRelation.Dataset.Name} unavailable");

                    if (!continueOnError)
                    {
                        throw;
                    }

                }

            }
        }

    }
}
