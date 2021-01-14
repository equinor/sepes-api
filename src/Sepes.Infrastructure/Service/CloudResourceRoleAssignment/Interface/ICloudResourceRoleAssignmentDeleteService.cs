using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceRoleAssignmentDeleteService
    {
        Task RemoveBySignatureAsync(int resourceId, string principalId, string roleId, bool failOnNotExist = false);
    }
}