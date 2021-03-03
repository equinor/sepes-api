using Moq;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Threading;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureNetworkSecurityGroupRuleMockServiceFactory
    {
        public static Mock<IAzureNetworkSecurityGroupRuleService> CreateWhereNetworkRulesArePositive() {

            var mock = new Mock<IAzureNetworkSecurityGroupRuleService>();          
            mock.Setup(us => us.IsRuleSetTo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RuleAction>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            return mock;

        }
    }
}
