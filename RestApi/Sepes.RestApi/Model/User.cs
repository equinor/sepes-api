using System;

namespace Sepes.RestApi.Model
{

    public class User
    {
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userGroup { get; set; }
        
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
