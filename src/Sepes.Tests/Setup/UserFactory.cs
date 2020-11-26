using Moq;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;

namespace Sepes.Tests.Setup
{
    public static class UserFactory
    {
        public static string OBJECT_ID = "objectId";
        public static string USERNAME = "testuser";
        public static string FULLNAME = "Test User";
        public static string EMAIL = "testuser@equinor.com";

        public static UserDto GetBasicAuthenticatedUser(int userId = 1)
        {
            var testUser = new UserDto(OBJECT_ID, USERNAME, FULLNAME, EMAIL);
            SetBasicUserProperties(testUser, userId);
            return testUser;
        }

        //public static UserDto GetUserWithStudyParticipants(List<StudyParticipantDto>)
        //{
        //    var testUser = new UserDto(OBJECT_ID, USERNAME, FULLNAME, EMAIL);
        //    testUser.StudyParticipants.AddRange();
        //    return testUser;
        //}

        public static UserDto GetAdmin(int userId = 1) {

            var testUser = new UserDto(OBJECT_ID, USERNAME, FULLNAME, EMAIL, admin: true);
            SetBasicUserProperties(testUser, userId);
            return testUser;
        }

        public static UserDto GetSponsor(int userId = 1)
        {
            var testUser = new UserDto(OBJECT_ID, USERNAME, FULLNAME, EMAIL, sponsor: true);
            SetBasicUserProperties(testUser, userId);
            return testUser;
        }

        public static UserDto GetDatasetAdmin(int userId = 1)
        {
            var testUser = new UserDto(OBJECT_ID, USERNAME, FULLNAME, EMAIL, datasetAdmin: true);
            SetBasicUserProperties(testUser, userId);
            return testUser;
        }

        public static void SetBasicUserProperties(UserDto user, int userId = 1)
        {
            user.Id = userId;

            if (user.Admin)
            {
                user.AppRoles.Add(AppRoles.Admin);
            }

            if (user.Sponsor)
            {
                user.AppRoles.Add(AppRoles.Sponsor);
            }

            if (user.DatasetAdmin)
            {
                user.AppRoles.Add(AppRoles.DatasetAdmin);
            }
        }

        public static Mock<IUserService> GetUserServiceMockForUserWithStudyRole(int userId = 1)
        {
            return SetupUserServiceMock(GetBasicAuthenticatedUser(userId));
        }

        public static Mock<IUserService> GetUserServiceMockForBasicUser(int userId = 1)
        {
            return SetupUserServiceMock(GetBasicAuthenticatedUser(userId));
        }

        public static Mock<IUserService> GetUserServiceMockForAdmin(int userId = 1)
        {
            return  SetupUserServiceMock(GetAdmin(userId));     
        }

        public static Mock<IUserService> GetUserServiceMockForSponsor(int userId = 1)
        {
            return SetupUserServiceMock(GetSponsor(userId));
        }

        public static Mock<IUserService> GetUserServiceForDatasetAdmin(int userId = 1)
        {
            return SetupUserServiceMock(GetDatasetAdmin(userId));
        }

        static Mock<IUserService> SetupUserServiceMock(UserDto user)
        {
            var mock = new Mock<IUserService>();
            mock.Setup(us => us.GetCurrentUser()).Returns(user);
            mock.Setup(us => us.GetCurrentUserFromDbAsync()).ReturnsAsync(user);
            mock.Setup(us => us.GetCurrentUserWithStudyParticipantsAsync()).ReturnsAsync(user);

            return mock;
        }       
    }
}
