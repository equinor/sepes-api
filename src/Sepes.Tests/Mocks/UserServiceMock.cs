using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks
{
    public class UserServiceMock : IUserService
    {
        public static string OBJECT_ID = "objectId";
        public static string USERNAME = "testuser";
        public static string FULLNAME = "Test User";
        public static string EMAIL = "testuser@equinor.com";
      

        UserDto GetBaseUser()
        {
            return new UserDto(OBJECT_ID, USERNAME, FULLNAME, EMAIL);
        }

        public UserDto GetCurrentUser()
        {
            var user = GetBaseUser();
            return user;
        }

        public async Task<UserDto> GetCurrentUserFromDbAsync()
        {
            var user = GetBaseUser();
            user.Id = 1;
            return user;
        }

        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            var dbUser = await GetCurrentUserFromDbAsync();
            //dbUser.StudyParticipants = new List<StudyParticipantDto>();
            //dbUser.StudyParticipants.Add(new StudyParticipantDto() {  UserId = 1, StudyId = 1, EmailAddress = dbUser.EmailAddress, FullName = dbUser.FullName, Role = StudyRoles.StudyOwner });
            return dbUser;
        }

        public Task<UserPermissionDto> GetUserPermissionsAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
