using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyPermissionService
    {
        Task<bool> HasAccessToOperationForStudy(IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null);
        bool HasAccessToOperationForStudy(UserDto currentUser, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null);
        void VerifyAccessOrThrow(SingleEntityDapperResult result, UserDto currentUser, UserOperation operation);
        Task VerifyAccessOrThrow(Study study, UserOperation operation, string roleBeingAddedOrRemoved = null);
    }
}
