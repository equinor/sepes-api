﻿using Xunit;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    [CollectionDefinition("Integration tests collection", DisableParallelization = true)]
    public class IntegrationTestsCollection : ICollectionFixture<TestHostFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
