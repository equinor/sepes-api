using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceRoleAssignmentCreateService
    {
        Task<CloudResourceRoleAssignmentDto> AddAsync(int resourceId, string principalId, string roleId);    
    }
}