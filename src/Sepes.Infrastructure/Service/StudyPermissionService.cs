using AutoMapper;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyPermissionService : IStudyPermissionService
    {       
        readonly IMapper _mapper;
        readonly IUserService _userService;

        public StudyPermissionService(IMapper mapper, IUserService userService)
        {          
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public bool HasAccessToOperationForStudy(UserDto currentUser, Study study, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var studyPermissionDetails = _mapper.Map<IHasStudyPermissionDetails>(study);
          
            return StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);
        }


        public bool HasAccessToOperationForStudy(UserDto currentUser, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            return StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);
        }

        public async Task<bool> HasAccessToOperationForStudy(IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            return StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);
        }

        public async Task<bool> HasAccessToOperationForStudy(Study study, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var studyPermissionDetails = _mapper.Map<IHasStudyPermissionDetails>(study);
           
            return StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);
        }

        void VerifyAccessOrThrow(UserDto currentUser, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            if (!HasAccessToOperationForStudy(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved))
            {
                throw AuthExceptionFactory.CreateForbiddenException(currentUser, studyPermissionDetails, operation);
            }
        }

        public void VerifyAccessOrThrow(SingleEntityDapperResult result, UserDto currentUser, UserOperation operation)
        {
            if (!result.Authorized)
            {
                throw AuthExceptionFactory.CreateForbiddenException(currentUser, result.StudyId, operation);
            }
        }

        public async Task VerifyAccessOrThrow(IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            VerifyAccessOrThrow(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);
        }

        public async Task VerifyAccessOrThrow(Study study, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var studyPermissionDetails = _mapper.Map<IHasStudyPermissionDetails>(study);

            VerifyAccessOrThrow(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);
        }


        public Study HasAccessToOperationForStudyOrThrow(UserDto currentUser, Study study, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var studyPermissionDetails = _mapper.Map<IHasStudyPermissionDetails>(study);
            VerifyAccessOrThrow(currentUser, studyPermissionDetails, operation, roleBeingAddedOrRemoved);

            return study;
        }
    }
}
