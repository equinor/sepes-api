using Sepes.Infrastructure.Model.Context;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    /// <summary>
    /// One instance of this will be created per test collection.
    /// </summary>
    public class TestHostFixture : ICollectionFixture<CustomWebApplicationFactory<Startup>>
    {
        public HttpClient Client;
        public SepesDbContext DbContext;
        public CustomWebApplicationFactory<Startup> factory;

        public TestHostFixture()
        {
            SetUserType(true);
        }

        public void SetUserType(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            factory = new CustomWebApplicationFactory<Startup>(isEmployee, isAdmin, isSponsor, isDatasetAdmin);
            Client = factory.CreateClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTest");
        }
    }
}
