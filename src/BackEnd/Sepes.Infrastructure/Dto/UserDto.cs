using Sepes.Infrastructure.Model.SepesSqlModels;
using System;

namespace Sepes.Infrastructure.Dto
{

    public class UserDto
    {
        public string userName { get; }
        public string userEmail { get; }
        public string userGroup { get; }
        
        public UserDto(string userName, string userEmail, string userGroup)
        {
            this.userName = userName;
            this.userEmail = userEmail;
            this.userGroup = userGroup;
        }

        public UserDB ToUserDB()
        {
            return new UserDB(){
                userName = userName,
                userEmail = userEmail,
                userGroup = userGroup
            };
        }

        public override bool Equals(object obj)
        {
            return obj is UserDto user && userEmail == user.userEmail;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(userEmail);
        }
    }

}
