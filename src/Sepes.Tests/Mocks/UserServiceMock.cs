using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks
{
    public class UserServiceMock : IUserService
    {
        public bool CurrentUserIsAdmin()
        {
            return true;
        }

        public UserDto GetBaseUser()
        {
            return new UserDto("testuser", "Test User", "testuser@equinor.com", "abcd", "objectId");
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
            dbUser.StudyParticipants = new List<StudyParticipantDto>();
            dbUser.StudyParticipants.Add(new StudyParticipantDto() {  UserId = 1, StudyId = 1, EmailAddress = dbUser.Email, FullName = dbUser.FullName, Role = StudyRoles.StudyOwner });
            return dbUser;
        }
    }
}
