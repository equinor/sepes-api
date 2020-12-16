using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxPhaseService : SandboxServiceBase, ISandboxPhaseService
    {       
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IAzureVNetService _azureVNetService;
        readonly IAzureStorageAccountService _azureStorageAccountService;


        public SandboxPhaseService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService, ISandboxResourceService sandboxResourceService, IAzureVNetService azureVNetService, IAzureStorageAccountService azureStorageAccountService)
            : base(config, db, mapper, logger, userService)
        {
        
            _sandboxResourceService = sandboxResourceService;
            _azureVNetService = azureVNetService;
            _azureStorageAccountService = azureStorageAccountService;
        }

        public async Task MoveToNextPhaseAsync(int sandboxId, CancellationToken cancellation = default)
        {
            _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Starting", sandboxId);

            SandboxPhaseHistory newestHistoryItem = null;

            try
            {
                var user = await _userService.GetCurrentUserAsync();

                var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Sandbox_IncreasePhase, true);

                var currentPhaseItem = SandboxPhaseUtil.GetCurrentPhaseHistoryItem(sandboxFromDb);

                var nextPhase = SandboxPhaseUtil.GetNextPhase(sandboxFromDb);

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Moving from {1} to {2}", sandboxId, currentPhaseItem.Phase, nextPhase);

                newestHistoryItem = new SandboxPhaseHistory() { Counter = currentPhaseItem.Counter + 1, Phase = nextPhase, CreatedBy = user.UserName };

                sandboxFromDb.PhaseHistory.Add(newestHistoryItem);
                await _db.SaveChangesAsync();
                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Phase added to db. Proceeding to make data available", sandboxId);

                await MakeDatasetsAvailable(sandboxId, cancellation);

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Done", sandboxId);
            }
            catch (Exception ex)
            {
                await AttemptRollback(sandboxId, newestHistoryItem);
                throw new Exception($"Moving to next phase failed", ex);
            }
        }

        async Task AttemptRollback(int sandboxId, SandboxPhaseHistory phaseToRemove)
        {        

            try
            {
                _logger.LogWarning($"Rolling back phase for sandbox {sandboxId}.");

                if (phaseToRemove != null)
                {
                    if (phaseToRemove.Id > 0)
                    {
                        var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Sandbox_IncreasePhase, true);

                        if(sandboxFromDb.PhaseHistory.Count > 0 && sandboxFromDb.PhaseHistory.Contains(phaseToRemove))
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

        async Task MakeDatasetsAvailable(int sandboxId, CancellationToken cancellation = default)
        {
            //Make data available
            //Connect storage account to vnet   
            var sandbox = await GetOrThrowAsync(sandboxId, UserOperation.Sandbox_IncreasePhase, true);

            var resourcesForSandbox = await _sandboxResourceService.GetSandboxResources(sandboxId, cancellation);

            var resourceGroupResource = SandboxResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.ResourceGroup, true);
            var vNetResource = SandboxResourceUtil.GetResourceByType(resourcesForSandbox, AzureResourceType.VirtualNetwork, true);

            if(resourceGroupResource == null)
            {
                throw new Exception($"Could not locate Resource Group entry for Sandbox {sandboxId}");
            }

            if (vNetResource == null)
            {
                throw new Exception($"Could not locate VNet entry for Sandbox {sandboxId}");
            }

            await _azureVNetService.EnsureSandboxSubnetHasServiceEndpointForStorage(resourceGroupResource.ResourceName, vNetResource.ResourceName);

            foreach (var curDatasetRelation in sandbox.SandboxDatasets)
            {
                if (curDatasetRelation.Dataset.StudyId.HasValue && curDatasetRelation.Dataset.StudyId == sandbox.StudyId)
                {
                    await MakeDatasetAvailable(sandbox.Study.StudySpecificDatasetsResourceGroup, curDatasetRelation.Dataset.StorageAccountName, resourceGroupResource.ResourceName, vNetResource.ResourceName, cancellation);
                }
                else
                {
                    throw new Exception($"Only study specific datasets are supported. Please remove dataset {curDatasetRelation.Dataset.Name} from Sandbox");
                }
            }
        }

        async Task MakeDatasetAvailable(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForSandbox, string vnetForSandbox, CancellationToken cancellation)
        {
            await _azureStorageAccountService.AddStorageAccountToVNet(resourceGroupForStorageAccount, storageAccountName, resourceGroupForSandbox, vnetForSandbox, cancellation);
        }
    }
}
