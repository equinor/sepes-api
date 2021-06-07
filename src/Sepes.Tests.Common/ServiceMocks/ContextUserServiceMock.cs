using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.RestApi.IntegrationTests.Services
{
    public class ContextUserServiceMock : IContextUserService
    {
        readonly bool _isEmployee;
        readonly bool _isAdmin;
        readonly bool _isSponsor;
        readonly bool _isDatasetAdmin;

        public ContextUserServiceMock(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            _isEmployee = isEmployee;
            _isAdmin = isAdmin;
            _isSponsor = isSponsor;
            _isDatasetAdmin = isDatasetAdmin;
        }

        public UserDto GetCurrentUser()
        {
           var user = new UserDto(TestUserConstants.COMMON_CUR_USER_OBJECTID, TestUserConstants.COMMON_CUR_USER_UPN, TestUserConstants.COMMON_CUR_USER_FULL_NAME, TestUserConstants.COMMON_CUR_USER_EMAIL,
                IsAdmin(), IsSponsor(), IsDatasetAdmin(), IsEmployee());

            ApplyExtendedProps(user);

            return user;
        }

        public bool IsEmployee()
        {
            return _isEmployee;
        }

        public bool IsAdmin()
        {
            return _isAdmin;
        }

        public bool IsSponsor()
        {
            return _isSponsor;
        }

        public bool IsDatasetAdmin()
        {
            return _isDatasetAdmin;
        }

        void ApplyExtendedProps(UserDto user)
        {
            if (IsAdmin())
            {
                user.Admin = true;
                user.AppRoles.Add(AppRoles.Admin);
            }

            if (IsSponsor())
            {
                user.Sponsor = true;
                user.AppRoles.Add(AppRoles.Sponsor);
            }

            if (IsDatasetAdmin())
            {
                user.DatasetAdmin = true;
                user.AppRoles.Add(AppRoles.DatasetAdmin);
            }

            if (IsEmployee())
            {
                user.Employee = true;
            }
        }

        public string GetCurrentUserObjectId()
        {
           return TestUserConstants.COMMON_CUR_USER_OBJECTID;
        }
    }
}
