using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto.Auth;
using Sepes.Infrastructure.Service.Azure.Interface;
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

        public async Task<AzureRoleAssignmentResponseDto> AddResourceRoleAssignment(string resourceId, string roleAssignmentId, string roleDefinitionId, string principalId, CancellationToken cancellationToken = default)
        {
            var addRoleUrl = $"https://management.azure.com{resourceId}/providers/Microsoft.Authorization/roleAssignments/{roleAssignmentId}?api-version=2015-07-01";

            var body = new AzureRoleAssignmentRequestDto(roleDefinitionId, principalId);
            var bodyJson = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var result = await PerformRequest<AzureRoleAssignmentResponseDto>(addRoleUrl, HttpMethod.Put, bodyJson, true, cancellationToken);

            return result;
        }
    }
}
