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

        [Fact]
        public void TestEqualsMethod()
        {
            var user1 = new User("Name1", "test@test.com", "sponsor");
            var sameAsUser1 = new User("Name1", "test@test.com", "sponsor");
            var differentUser = new User("Name2", "test2@test.com", "sponsor2");

            Assert.True(user1.Equals(sameAsUser1));
            Assert.False(user1.Equals(differentUser));
        }
    }
}
