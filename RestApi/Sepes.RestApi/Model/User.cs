using System;

namespace Sepes.RestApi.Model
{

    public class User
    {
        public string userName { get; }
        public string userEmail { get; }
        public string userGroup { get; }
        
        public User(string userName, string userEmail, string userGroup)
        {
            this.userName = userName;
            this.userEmail = userEmail;
            this.userGroup = userGroup;
        }

        public override bool Equals(object obj)
        {
            return obj is User user && userEmail == user.userEmail;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(userEmail);
        }
    }

}
