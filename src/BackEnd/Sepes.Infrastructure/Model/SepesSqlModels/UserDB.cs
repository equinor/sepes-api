
using Sepes.Infrastructure.Dto;

namespace Sepes.Infrastructure.Model.SepesSqlModels
{
    public class UserDB
    {
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userGroup { get; set; }

        public UserDto ToUser() => new UserDto(userName, userEmail, userGroup);
    }
}
