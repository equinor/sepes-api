using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.IntegrationTests.Setup
{
    public class IntegrationTestAzureADUService
    {
        public Task<User> GetUser(string id)
        {
            var user = new User { Surname = "Test", GivenName = "Test", Id = Guid.NewGuid().ToString(), DisplayName = " Test Test", Mail = "Test@Test.com" };
            return Task.FromResult(user);
        }

        public Task<List<User>> SearchUsers(string search, int limit)
        {
            var userList = new List<User>();
            userList.Add(new User { Surname = "Test", GivenName = "Test", Id = Guid.NewGuid().ToString(), DisplayName = " Test Test", Mail = "Test@Test.com" });
            return Task.FromResult(userList);
        }
    }
}
