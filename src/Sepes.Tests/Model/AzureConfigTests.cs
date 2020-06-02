//using Sepes.RestApi.Model;
//using Xunit;

//namespace Sepes.Tests.Model
//{
//    public class AzureConfigTests
//    {
//        [Fact]
//        public void Constructor()
//        {
//            var config = new AzureConfig(
//                "a540dd43-198f-4f46-ba9c-e14f9da0c1bd",
//                "de81f3d2-351f-44ea-a342-46b6238d7d13",
//                "Secret",
//                "97b3bb1f-647a-4a6e-b10b-b78cd8c3e093",
//                "Test-CommonSepes",
//                "ExampleJoinNetwork"
//                );
//            Assert.Equal("a540dd43-198f-4f46-ba9c-e14f9da0c1bd", config.credentials.TenantId);
//            Assert.Equal("de81f3d2-351f-44ea-a342-46b6238d7d13", config.credentials.ClientId);
//            Assert.Equal("97b3bb1f-647a-4a6e-b10b-b78cd8c3e093", config.credentials.DefaultSubscriptionId);
//            Assert.Equal("Test-CommonSepes", config.commonGroup);
//            Assert.Equal("ExampleJoinNetwork", config.joinNetworkRoleName);
//        }
//    }
//}
