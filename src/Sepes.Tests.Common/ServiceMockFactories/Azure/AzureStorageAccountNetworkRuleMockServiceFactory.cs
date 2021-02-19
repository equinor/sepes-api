using Moq;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Collections.Generic;
using System.Threading;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureStorageAccountNetworkRuleMockServiceFactory
    {
        public static Mock<IAzureStorageAccountNetworkRuleService> CreateBasicForCreate() {

            var mock = new Mock<IAzureStorageAccountNetworkRuleService>();
            mock.Setup(us => us.AddStorageAccountToVNet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            mock.Setup(us => us.SetFirewallRules(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<FirewallRule>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string a, string b, List<FirewallRule> c, CancellationToken cancellation) => c );

            return mock;
        }
    }
}
