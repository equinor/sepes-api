using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceRoleAssignmentUpdateService
    {
        Task<CloudResourceRoleAssignmentDto> SetForeignIdAsync(int roleAssignmentId, string foreignSystemId);    
    }
}