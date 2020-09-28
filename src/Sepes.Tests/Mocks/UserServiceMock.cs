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

        public bool CurrentUserIsDatasetAdmin()
        {
            return true;
        }

        public bool CurrentUserIsSponsor()
        {
            return true;
        }

        public UserDto GetBaseUser()
        {
            return new UserDto("objectId", "testuser", "Test User", "testuser@equinor.com");
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
            dbUser.StudyParticipants.Add(new StudyParticipantDto() {  UserId = 1, StudyId = 1, EmailAddress = dbUser.EmailAddress, FullName = dbUser.FullName, Role = StudyRoles.StudyOwner });
            return dbUser;
        }
    }
}
