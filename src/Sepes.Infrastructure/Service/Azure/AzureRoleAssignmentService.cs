using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto.Auth;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
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

        async Task<AzureRoleAssignmentResponseDto> GetById(string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            var getRoleUrl = CreateLinkForExistingRoleAssignment(roleAssignmentId);
            var result = await PerformRequest<AzureRoleAssignmentResponseDto>(getRoleUrl, HttpMethod.Get, needsAuth: true, cancellationToken: cancellationToken);

            return result;
        }       

        public async Task<AzureRoleAssignmentResponseDto> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(roleAssignmentId))
            {
                roleAssignmentId = Guid.NewGuid().ToString();
            }

            var addRoleUrl = $"https://management.azure.com{resourceId}/providers/Microsoft.Authorization/roleAssignments/{roleAssignmentId}?api-version=2015-07-01";

            var body = new AzureRoleAssignmentRequestDto(roleDefinitionId, principalId);
            var bodyJson = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var result = await PerformRequest<AzureRoleAssignmentResponseDto>(addRoleUrl, HttpMethod.Put, bodyJson, true, cancellationToken);

            return result;
        }

        public async Task<AzureRoleAssignmentResponseDto> DeleteRoleAssignment(string roleAssignmentId, CancellationToken cancellationToken = default)
        {
            var getRoleUrl = CreateLinkForExistingRoleAssignment(roleAssignmentId);
            var result = await PerformRequest<AzureRoleAssignmentResponseDto>(getRoleUrl, HttpMethod.Delete, needsAuth: true, cancellationToken: cancellationToken);

            return result;
        }

        string CreateLinkForExistingRoleAssignment(string roleAssignmentId)
        {
            return $"https://management.azure.com{roleAssignmentId}?api-version=2015-07-01";
        }
    }
}
