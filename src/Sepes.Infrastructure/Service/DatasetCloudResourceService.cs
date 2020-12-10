using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.Auth;
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
    public class DatasetCloudResourceService : IDatasetCloudResourceService
    {
        readonly IConfiguration _config;
        readonly SepesDbContext _db;
        readonly ILogger<DatasetCloudResourceService> _logger;

        readonly IUserService _userService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureStorageAccountService _storageAccountService;
        readonly IAzureRoleAssignmentService _roleAssignmentService;

        public DatasetCloudResourceService(IConfiguration config, SepesDbContext db, ILogger<DatasetCloudResourceService> logger,
           IUserService userService, IAzureResourceGroupService resourceGroupService, IAzureStorageAccountService storageAccountService, IAzureRoleAssignmentService roleAssignmentService)
        {
            _config = config;
            _db = db;
            _logger = logger;
            _userService = userService;
            _storageAccountService = storageAccountService;
            _resourceGroupService = resourceGroupService;
            _roleAssignmentService = roleAssignmentService;
        }

        public async Task CreateResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

            try
            {
                await CreateStorageAccountForStudySpecificDatasets(study, dataset, cancellationToken);
                await AddRoleAssignmentForCurrentUser(dataset, cancellationToken);
            }
            catch (Exception ex)
            {               
                throw new Exception($"Failed to create resources for study specific dataset", ex);
            }

        }

        public async Task DeleteResourcesForStudySpecificDatasetAsync(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"DeleteResourcesForStudySpecificDatasetAsync - Dataset Id: {dataset.Id}");

            try
            {
                if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup) == false && String.IsNullOrWhiteSpace(dataset.StorageAccountName) == false)
                {
                    await _storageAccountService.DeleteStorageAccount(study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, cancellationToken);
                }
              
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete resources for study specific dataset", ex);
            }
        }

        public async Task DeleteAllStudyRelatedResourcesAsync(Study study, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"DeleteAllStudyRelatedResourcesAsync - Study Id: {study.Id}");

            try
            {
                if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup) == false)
                {
                    await _resourceGroupService.Delete(study.StudySpecificDatasetsResourceGroup, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete resources for study specific dataset", ex);
            }
        }

        async Task CreateStorageAccountForStudySpecificDatasets(Study study, Dataset dataset, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"CreateResourcesForStudySpecificDataset - Dataset Id: {dataset.Id}");

            if (String.IsNullOrWhiteSpace(study.StudySpecificDatasetsResourceGroup))
            {
                study.StudySpecificDatasetsResourceGroup = AzureResourceNameUtil.StudySpecificDatasetResourceGroup(study.Name);
            }

            var tags = AzureResourceTagsFactory.StudySpecificDatasourceResourceGroupTags(_config, study);

            await _resourceGroupService.EnsureCreated(study.StudySpecificDatasetsResourceGroup, RegionStringConverter.Convert(dataset.Location), tags, cancellationToken);

            //var allowAccessFrom = dataset.FirewallRules != null && dataset.FirewallRules.Count > 0 ? dataset.FirewallRules.Select(fw => fw.Address).ToList() : null;
            var newStorageAccount = await _storageAccountService.CreateStorageAccount(RegionStringConverter.Convert(dataset.Location), study.StudySpecificDatasetsResourceGroup, dataset.StorageAccountName, tags, onlyAllowAccessFrom: null, cancellationToken);

            dataset.StorageAccountId = newStorageAccount.Id;
            dataset.StorageAccountName = newStorageAccount.Name;
            await _db.SaveChangesAsync();

            //Todo: Create firewall rule
        }

        async Task AddRoleAssignmentForCurrentUser(Dataset dataset, CancellationToken cancellationToken = default)
        {
            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            var roleAssignmentId = Guid.NewGuid().ToString();
            var roleDefinitionId = $"{dataset.StorageAccountId}/providers/Microsoft.Authorization/roleDefinitions/{AzureRoleDefinitionId.READ}";
            await _roleAssignmentService.AddResourceRoleAssignment(dataset.StorageAccountId, roleAssignmentId, roleDefinitionId, currentUser.ObjectId, cancellationToken);
        }
    }
}
