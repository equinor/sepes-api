using Sepes.Infrastructure.Model.Context;
using Sepes.RestApi;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace Sepes.API.IntegrationTests.Setup
{
    /// <summary>
    /// One instance of this will be created per test collection.
    /// </summary>
    public class TestHostFixture : ICollectionFixture<CustomWebApplicationFactory<Startup>>
    {
        public readonly HttpClient Client;
        public readonly SepesDbContext DbContext;
        public readonly CustomWebApplicationFactory<Startup> factory;

        public TestHostFixture()
        {
            factory = new CustomWebApplicationFactory<Startup>();
            Client = factory.CreateClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTest");
        }
    }
}
