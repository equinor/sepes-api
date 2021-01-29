﻿using Sepes.Infrastructure.Interface;
using System;
using Sepes.RestApi.IntegrationTests.TestHelpers;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public class IntegrationTestUserService : ICurrentUserService
    {
        public string GetUserId()
        {
            return TestConstants.TestUserGuid;
        }      
    }
}
