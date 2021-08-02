using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto.RoleAssignment;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureRoleAssignmentService : AzureApiServiceBase, IAzureRoleAssignmentService 
    {
        const string API_VERSION_ARGUMENT = "api-version=2018-07-01";
        readonly string _subscriptionId;

        public AzureRoleAssignmentService(IConfiguration config, ILogger<AzureRoleAssignmentService> logger, IAzureApiRequestAuthenticatorService azureApiRequestAuthenticatorService, HttpClient httpClient)
            : base(config, logger, azureApiRequestAuthenticatorService, httpClient)
        {
            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];
        }

        public async Task<bool> RoleAssignmentExists(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingAssignment = await GetById(roleAssignmentId, cancellationToken);
                return existingAssignment != null;

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Role assignment with id {roleAssignmentId} for resource {resourceId} does not appear to exist");
            }

            return false;
        }

        public async Task<AzureRoleAssignment> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(roleAssignmentId))
                {
                    roleAssignmentId = Guid.NewGuid().ToString();
                }

                var addRoleUrl = $"https://management.azure.com{resourceId}/providers/Microsoft.Authorization/roleAssignments/{roleAssignmentId}?{API_VERSION_ARGUMENT}";

                var body = new AzureRoleAssignmentRequestDto(roleDefinitionId, principalId);
                var bodyJson = new StringContent(JsonSerializerUtil.Serialize(body), Encoding.UTF8, "application/json");

                var result = await PerformRequest<AzureRoleAssignment>(addRoleUrl, HttpMethod.Put, bodyJson, true, cancellationToken: cancellationToken);

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception($"Add role assignment with id {roleAssignmentId} failed for resource {resourceId}. Role definition: {roleDefinitionId}, principalId: {principalId}", ex);
            }
        }

        public async Task<AzureRoleAssignment> DeleteRoleAssignment(string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            var getRoleUrl = CreateLinkForExistingRoleAssignment(roleAssignmentId);
            var result = await PerformRequest<AzureRoleAssignment>(getRoleUrl, HttpMethod.Delete, needsAuth: true, cancellationToken: cancellationToken);

            return result;
        }
        
        async Task<AzureRoleAssignment> GetById(string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            var getRoleUrl = CreateLinkForExistingRoleAssignment(roleAssignmentId);
            var result = await PerformRequest<AzureRoleAssignment>(getRoleUrl, HttpMethod.Get, needsAuth: true, cancellationToken: cancellationToken);

            return result;
        }

        string CreateLinkForExistingRoleAssignment(string roleAssignmentId)
        {
            return $"https://management.azure.com{roleAssignmentId}?{API_VERSION_ARGUMENT}";
        }

        public async Task<List<AzureRoleAssignment>> GetResourceGroupRoleAssignments(string resourceGroupId, string resourceGroupName, CancellationToken cancellation = default)
        {
            var url = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/roleAssignments";
            var assignmentsFromAzure = await GetRoleAssignments(url, cancellation);

            var filteredRoleAssignments = assignmentsFromAzure.value.Where(ra => ra.properties.scope.Contains($"/resourceGroups/{resourceGroupId}") || ra.properties.scope.Contains($"/resourceGroups/{resourceGroupName}")).ToList();

            return filteredRoleAssignments;
        }

        public async Task<List<AzureRoleAssignment>> GetStorageAccountRoleAssignments(string resourceGroupId, string resourceGroupName, string storageAccountId, string storageAccountName, CancellationToken cancellation = default)
        {
            var url = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Storage/storageAccounts/{storageAccountName}/providers/Microsoft.Authorization/roleAssignments";
            var assignmentsFromAzure = await GetRoleAssignments(url, cancellation);        
            var filteredRoleAssignments = assignmentsFromAzure.value.Where(ra => ra.properties.scope.Contains($"/storageAccounts/{storageAccountId}") || ra.properties.scope.Contains($"/storageAccounts/{storageAccountName}")).ToList();

            return filteredRoleAssignments;
        }

        async Task<RoleAssignmentResponse> GetRoleAssignments(string url, CancellationToken cancellation = default)
        {          
            var completeUrl = $"{url}?{API_VERSION_ARGUMENT}&$filter=atScope()";
            var assignmentsFromAzure = await PerformRequest<RoleAssignmentResponse>(completeUrl, HttpMethod.Get, needsAuth: true, cancellationToken: cancellation);

            return assignmentsFromAzure;
        }      
    }
}
