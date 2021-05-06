using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure.RoleAssignment;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureRoleAssignmentService : AzureApiServiceBase, IAzureRoleAssignmentService
    {
        public AzureRoleAssignmentService(IConfiguration config, ILogger<AzureCostManagementService> logger, ITokenAcquisition tokenAcquisition) : base(config, logger, tokenAcquisition)
        {

        }

        string CreateRoleAssignmentErrorString(List<AzureRoleAssignment> azureRoleAssignments)
        {
            var sb = new StringBuilder();
            azureRoleAssignments.ForEach(ra => sb.AppendLine($"{ra.id} | {ra.properties.principalId} | {ra.properties.roleDefinitionId} "));
            return sb.ToString();
        }

        string CreateConfigErrorString(HashSet<string> filter)
        {
            var sb = new StringBuilder();

            foreach(var cur in filter)
            {
                sb.AppendLine(cur);
            }

            return sb.ToString();
        }

        public async Task SetRoleAssignments(string resourceGroupId, string resourceGroupName, List<CloudResourceDesiredRoleAssignmentDto> desiredRoleAssignments, CancellationToken cancellationToken = default)
        {
            var createdByFilter = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_config, ConfigConstants.ROLE_ASSIGNMENTS_MANAGED_BY);         

            _logger.LogInformation($"SetRoleAssignments: Filtering by {CreateConfigErrorString(createdByFilter)}!");

            var existingRoleAssignments = await GetResourceGroupRoleAssignments(resourceGroupId, resourceGroupName, createdByFilter, cancellationToken);

            //Create desired roles that does not allready exist
            foreach (var curDesired in desiredRoleAssignments)
            {
                try
                {
                    var sameRoleFromExisting = existingRoleAssignments.Where(ra => ra.properties.principalId == curDesired.PrincipalId && ra.properties.roleDefinitionId.Contains(curDesired.RoleId)).FirstOrDefault();

                    if (sameRoleFromExisting != null)
                    {
                        _logger.LogInformation($"Principal {curDesired.PrincipalId} allready had role {curDesired.RoleId}");
                    }
                    else
                    {
                        var roleDefinitionId = AzureRoleIds.CreateRoleDefinitionUrl(resourceGroupId, curDesired.RoleId);
                        _logger.LogInformation($"Principal {curDesired.PrincipalId} missing role {curDesired.RoleId}, creating. Role definition: {roleDefinitionId}");
                        await AddRoleAssignment(resourceGroupId, roleDefinitionId, curDesired.PrincipalId, cancellationToken: cancellationToken);
                    }

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("RoleAssignmentExists"))
                    {
                        var existingRoleAssignmentsAfterError = await GetResourceGroupRoleAssignments(resourceGroupId, resourceGroupName, createdByFilter, cancellationToken);

                        _logger.LogError(ex, $"It appears that role assignment allready exist. Initial list: {CreateRoleAssignmentErrorString(existingRoleAssignments)}. Updated list: {CreateRoleAssignmentErrorString(existingRoleAssignmentsAfterError)}");
                        continue;
                    }


                    throw;
                }
            }

            //Find out what roles are allready in place, and delete those that are no longer needed
            foreach (var curExisting in existingRoleAssignments)
            {
                var curExistingRoleId = AzureRoleIds.GetRoleIdFromDefinition(curExisting.properties.roleDefinitionId);

                CloudResourceDesiredRoleAssignmentDto sameRoleFromDesired = null;

                if (curExistingRoleId != null)
                {
                    sameRoleFromDesired = desiredRoleAssignments.Where(ra => ra.PrincipalId == curExisting.properties.principalId && ra.RoleId == curExistingRoleId).FirstOrDefault();
                }

                if (sameRoleFromDesired != null)
                {
                    _logger.LogInformation($"Existing role for principal {curExisting.properties.principalId} with id {curExisting.properties.roleDefinitionId} also in desired role list. Keeping");
                }
                else
                {
                    _logger.LogInformation($"Existing role for principal {curExisting.properties.principalId} with id {curExisting.properties.roleDefinitionId} NOT in desired role list. Will be deleted");
                    await DeleteRoleAssignment(curExisting.id, cancellationToken);
                }
            }
        }

        async Task<bool> RoleAssignmentExists(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default)
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

        async Task<AzureRoleAssignment> GetById(string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            var getRoleUrl = CreateLinkForExistingRoleAssignment(roleAssignmentId);
            var result = await PerformRequest<AzureRoleAssignment>(getRoleUrl, HttpMethod.Get, needsAuth: true, cancellationToken: cancellationToken);

            return result;
        }

        async Task<AzureRoleAssignment> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default)
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

        async Task<AzureRoleAssignment> DeleteRoleAssignment(string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            var getRoleUrl = CreateLinkForExistingRoleAssignment(roleAssignmentId);
            var result = await PerformRequest<AzureRoleAssignment>(getRoleUrl, HttpMethod.Delete, needsAuth: true, cancellationToken: cancellationToken);

            return result;
        }

        string CreateLinkForExistingRoleAssignment(string roleAssignmentId)
        {
            return $"https://management.azure.com{roleAssignmentId}?api-version=2015-07-01";
        }

        async Task<List<AzureRoleAssignment>> GetResourceGroupRoleAssignments(string resourceGroupId, string resourceGroupName, HashSet<string> createdByFilter = default, CancellationToken cancellation = default)
        {
            var url = $"https://management.azure.com/subscriptions/{_subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01&$filter=atScope()";
            var assignmentsFromAzure = await PerformRequest<RoleAssignmentResponse>(url, HttpMethod.Get, needsAuth: true, cancellationToken: cancellation);

            var result = new List<AzureRoleAssignment>();

            var filteredRoleAssignments = assignmentsFromAzure.value.Where(ra => ra.properties.scope.Contains($"/resourceGroups/{resourceGroupId}") || ra.properties.scope.Contains($"/resourceGroups/{resourceGroupName}")).ToList();

            foreach (var curAssignment in filteredRoleAssignments)
            {              
                //Only those created by the principles in the list
                if (createdByFilter == null || createdByFilter.Contains(curAssignment.properties.createdBy))
                {
                    _logger.LogInformation($"GetResourceGroupRoleAssignments: Including {curAssignment.properties.principalId}, {curAssignment.properties.roleDefinitionId}");
                    result.Add(curAssignment);
                }
                else
                {
                    _logger.LogInformation($"GetResourceGroupRoleAssignments: Excluding {curAssignment.properties.principalId}, {curAssignment.properties.roleDefinitionId}");
                }
            }

            return result;
        }
    }
}
