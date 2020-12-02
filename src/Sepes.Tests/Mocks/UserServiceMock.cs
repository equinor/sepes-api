using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks
{
    public class UserServiceMock : IUserService
    {
        UserDto GetBaseUser()
        {
            var testUser = UserFactory.GetAdmin(1);        

            return testUser;
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
