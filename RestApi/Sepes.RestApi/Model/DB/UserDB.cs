
namespace Sepes.RestApi.Model
{
    public class UserDB
    {
        public string userName { get; set; }
        public string userEmail { get; set; }
        public string userGroup { get; set; }

        public User ToUser() => new User(userName, userEmail, userGroup);
    }
}
