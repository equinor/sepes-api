using Sepes.RestApi.Model;
using Xunit;

namespace Sepes.RestApi.Tests.Model
{
    public class UserTests
    {
        [Fact]
        public void Constructor()
        {
            var user = new User("testuser", "testemail@example.com", "MainGroup");

            Assert.Equal("testuser", user.userName);
            Assert.Equal("testemail@example.com", user.userEmail);
            Assert.Equal("MainGroup", user.userGroup);
        }
    }
}
