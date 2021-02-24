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
        public CustomWebApplicationFactory<Startup> Factory;

        public TestHostFixture()
        {
            SetScenario();
        }

        public void SetScenario(IMockServicesForScenarioProvider mockServicesForScenarioProvider = null, bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            Factory = new CustomWebApplicationFactory<Startup>(mockServicesForScenarioProvider, isEmployee, isAdmin, isSponsor, isDatasetAdmin);       
            Client = Factory.CreateClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTest");
        }
    }
}
