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

            foreach (var curAssignment in assignmentsFromAzure.value)
            {
                //Only those set at resource group level, not inherited one
                if (curAssignment.properties.scope.Contains($"/resourceGroups/{resourceGroupId}") || curAssignment.properties.scope.Contains($"/resourceGroups/{resourceGroupName}"))
                {
                    //Only those created by the principles in the list
                    if (createdByFilter == null || createdByFilter.Contains(curAssignment.properties.createdBy))
                    {
                        result.Add(curAssignment);
                    }

                }
            }

            return result;
        }

        public async Task SetRoleAssignments(string resourceGroupId, string resourceGroupName, List<CloudResourceDesiredRoleAssignmentDto> desiredRoleAssignments, CancellationToken cancellationToken = default)
        {
            var createdByFilter = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_config, ConfigConstants.ROLE_ASSIGNMENTS_MANAGED_BY);

            var existingRoleAssignments = await GetResourceGroupRoleAssignments(resourceGroupId, resourceGroupName, createdByFilter, cancellationToken);
            
            //Create desired roles that does not allready exist
            foreach (var curDesired in desiredRoleAssignments)
            {
                var sameRoleFromExisting = existingRoleAssignments.Where(ra=> ra.properties.principalId == curDesired.PrincipalId && ra.properties.roleDefinitionId.Contains(curDesired.RoleId)).FirstOrDefault();
                               
                if (sameRoleFromExisting != null)
                {
                    _logger.LogInformation($"Principal {curDesired.PrincipalId} allready had role {curDesired.RoleId}");
                }
                else
                {
                    _logger.LogInformation($"Principal {curDesired.PrincipalId} missing role {curDesired.RoleId}, creating");
                    var roleDefinitionId = AzureRoleIds.CreateRoleDefinitionUrl(resourceGroupId, curDesired.RoleId);
                    await AddRoleAssignment(resourceGroupId, roleDefinitionId, curDesired.PrincipalId, cancellationToken: cancellationToken);
                }
            }

            //Find out what roles are allready in place, and delete those that are no longer needed
            foreach (var curExisting in existingRoleAssignments)
            {
                var curExistingRoleId = AzureRoleIds.GetRoleIdFromDefinition(curExisting.properties.roleDefinitionId);
                
                CloudResourceDesiredRoleAssignmentDto sameRoleFromDesired = null;

                if (curExistingRoleId != null)
                {
                    sameRoleFromDesired = desiredRoleAssignments.Where(ra => ra.PrincipalId == curExisting.properties.principalId&& ra.RoleId == curExistingRoleId).FirstOrDefault();
                }
                
                if(sameRoleFromDesired != null)
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
    }
}
