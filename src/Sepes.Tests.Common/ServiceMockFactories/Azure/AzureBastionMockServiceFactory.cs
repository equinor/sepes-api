using Moq;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Tests.Common.ServiceMockFactories.Azure;
using System.Threading;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureBastionMockServiceFactory
    {
        public static Mock<IAzureBastionService> CreateBasicForCreate() {

            var mock = new Mock<IAzureBastionService>();          
            mock.Setup(us => us.EnsureCreated(It.IsAny<ResourceProvisioningParameters>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync((ResourceProvisioningParameters ipt, CancellationToken cancellation) => ProvisioningResultFactory.Create(ipt, AzureResourceType.Bastion));

            return mock;

        }
    }
}
