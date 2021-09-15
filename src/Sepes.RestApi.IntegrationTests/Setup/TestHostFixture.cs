using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    /// <summary>
    /// One instance of this will be created per test collection.
    /// </summary>
    public class TestHostFixture : ICollectionFixture<CustomWebApplicationFactory<Startup>>
    {
        readonly Dictionary<Tuple<bool, bool, bool, bool>,
          RestHelper> _scenarios = new Dictionary<Tuple<bool, bool, bool, bool>, RestHelper>();        

        public TestHostFixture()
        {         

        }        
        
        public RestHelper GetRestHelperForScenario(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            return EnsureScenarioExists(isEmployee, isAdmin, isSponsor, isDatasetAdmin);
        }       

        RestHelper EnsureScenarioExists(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            var tuple = CreateScenarioTuple(isEmployee, isAdmin, isSponsor, isDatasetAdmin);

            if (!_scenarios.TryGetValue(tuple, out RestHelper restHelper))
            {
                var factory = new CustomWebApplicationFactory<Startup>(isEmployee, isAdmin, isSponsor, isDatasetAdmin);
                var client = factory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IntegrationTest");

                restHelper = new RestHelper(client);
                _scenarios.Add(tuple, restHelper);
            }

            return restHelper;
        }       

        Tuple<bool, bool, bool, bool> CreateScenarioTuple(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            return new Tuple<bool, bool, bool, bool>(isEmployee, isAdmin, isSponsor, isDatasetAdmin);
        }
    }
}
