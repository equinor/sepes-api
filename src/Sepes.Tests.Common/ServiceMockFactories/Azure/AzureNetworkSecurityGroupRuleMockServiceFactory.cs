using Moq;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Threading;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureNetworkSecurityGroupRuleMockServiceFactory
    {
        public static Mock<IAzureNetworkSecurityGroupRuleService> CreateWhereRuleSetToReturnsTrue() {

            var mock = new Mock<IAzureNetworkSecurityGroupRuleService>();          
            mock.Setup(us => us.IsRuleSetTo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RuleAction>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(true);

            return mock;

        }

        public static Mock<IAzureNetworkSecurityGroupRuleService> CreateWhereRuleSetToReturnsFalse()
        {

            var mock = new Mock<IAzureNetworkSecurityGroupRuleService>();
            mock.Setup(us => us.IsRuleSetTo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RuleAction>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(false);

            return mock;

        }
    }
}
