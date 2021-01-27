using Sepes.Infrastructure.Interface;
using System;
using Sepes.RestApi.IntegrationTests.TestHelpers;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public class IntegrationTestUserService : ICurrentUserService
    {

        public string GetUserFullName()
        {
            return "Mr Integration Test";
        }

        public string GetUserGuid()
        {
            return TestConstants.TestUserGuid;
        }

        public string GetUserId()
        {
            return TestConstants.TestUserGuid;
        }

        public string GetUsername()
        {
            return "test_user@integrationtesting.com";
        }
    }
}
