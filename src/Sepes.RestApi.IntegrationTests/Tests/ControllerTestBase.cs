using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class ControllerTestBase : IAsyncLifetime
    {
        protected readonly TestHostFixture _testHostFixture;
        protected RestHelper _restHelper;

        public ControllerTestBase(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
            _restHelper = new RestHelper(testHostFixture.Client);
        }

        protected void SetScenario(IMockServicesForScenarioProvider mockServicesForScenarioProvider = null, bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            _testHostFixture.SetScenario(mockServicesForScenarioProvider, isEmployee, isAdmin, isSponsor, isDatasetAdmin);
            _restHelper = new RestHelper(_testHostFixture.Client);
        }

        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;

        protected async Task WithBasicSeeds()
        {
            await SeedRegions();
        }        

        protected static async Task SeedRegions()
        {
            var region = new Infrastructure.Model.Region()
            {
                Created = DateTime.UtcNow,
                CreatedBy = "seed",
                Key = "norwayeast",
                Name = "Norway East",
                DiskSizeAssociations = new List<Infrastructure.Model.RegionDiskSize>() {
                    new Infrastructure.Model.RegionDiskSize(){ DiskSize = new Infrastructure.Model.DiskSize() { Key = "standardssd-e1", DisplayText = "4 GB", Size = 4 } },
                    new Infrastructure.Model.RegionDiskSize(){ DiskSize = new Infrastructure.Model.DiskSize() { Key = "standardssd-e2", DisplayText = "8 GB", Size = 8 } }
                },
                VmSizeAssociations = new List<Infrastructure.Model.RegionVmSize>() {
                    new Infrastructure.Model.RegionVmSize(){ VmSize = new Infrastructure.Model.VmSize() { Key = "Standard_F1" } },
                    new Infrastructure.Model.RegionVmSize(){ VmSize = new Infrastructure.Model.VmSize() { Key = "Standard_F2" } },
                }
            };

            await SliceFixture.InsertAsync(region);
        }

        protected async Task<ApiResponseWrapper> ProcessWorkQueue()
        {
            //SetUserType(isAdmin: true); //If this test will be ran as non-admins, must find a way to set admin before running this

            var responseWrapper = await _restHelper.Get("api/provisioningqueue/lookforwork");

            Assert.Equal(System.Net.HttpStatusCode.OK, responseWrapper.StatusCode);

            return responseWrapper;
        }
    }
}
