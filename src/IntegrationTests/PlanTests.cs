using Sepes.Infrastructure.Dto.VirtualMachine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;
using Sepes.IntegrationTests;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.IntegrationTests.Setup;

namespace Sepes.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class PlanTests : IAsyncLifetime
    {
        private const string _endpoint = "/validateUsername";
        private readonly RestHelper _restHelper;

        public PlanTests(TestHostFixture testHostFixture)
        {
            _restHelper = new RestHelper(testHostFixture.Client);
        }

        [Fact]
        public async Task GetAllPlans()
        {
            //Arrange
            await SliceFixture.InsertAsync(new VmUsernameDto { username = "First name" });
            await SliceFixture.InsertAsync(new VmUsernameDto { username = "Second name" });

            //Act
            var result = await _restHelper.Get<ReadOnlyCollection<VmUsernameValidateDto>>(_endpoint);

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.isValid.Equals(true));
        }

        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
