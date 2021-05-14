using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sepes.Azure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto.RoleAssignment;
using Sepes.Common.Service;
using Sepes.Common.Constants;

namespace Sepes.Azure.Service
{
    public class AzureRoleAssignmentService : RestApiServiceBase, IAzureRoleAssignmentService 
    {
        readonly string _subscriptionId;

        public AzureRoleAssignmentService(IConfiguration config, ILogger<AzureRoleAssignmentService> logger, ITokenAcquisition tokenAcquisition)
            : base(config, logger, tokenAcquisition)
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

                var addRoleUrl = $"https://management.azure.com{resourceId}/providers/Microsoft.Authorization/roleAssignments/{roleAssignmentId}?api-version=2015-07-01";

                var body = new AzureRoleAssignmentRequestDto(roleDefinitionId, principalId);
                var bodyJson = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                var result = await PerformRequest<AzureRoleAssignment>(addRoleUrl, HttpMethod.Put, bodyJson, true, cancellationToken);

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
            return $"https://management.azure.com{roleAssignmentId}?api-version=2015-07-01";
        }

        public async Task<List<AzureRoleAssignment>> GetResourceGroupRoleAssignments(string resourceGroupId, string resourceGroupName, CancellationToken cancellation = default)
        {
            var url = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01&$filter=atScope()";
            var assignmentsFromAzure = await PerformRequest<RoleAssignmentResponse>(url, HttpMethod.Get, needsAuth: true, cancellationToken: cancellation);
 
            var filteredRoleAssignments = assignmentsFromAzure.value.Where(ra => ra.properties.scope.Contains($"/resourceGroups/{resourceGroupId}") || ra.properties.scope.Contains($"/resourceGroups/{resourceGroupName}")).ToList();

            return filteredRoleAssignments;
        }
    }
}
