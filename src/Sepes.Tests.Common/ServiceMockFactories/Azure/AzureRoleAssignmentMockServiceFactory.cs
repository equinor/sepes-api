using Moq;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Collections.Generic;
using System.Threading;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureRoleAssignmentMockServiceFactory
    {
        public static Mock<IAzureRoleAssignmentService> CreateBasicForCreate() {

            var mock = new Mock<IAzureRoleAssignmentService>();

            mock.Setup(us => us.SetRoleAssignments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<CloudResourceDesiredRoleAssignmentDto>>(), It.IsAny<CancellationToken>()));

            return mock;

        }
    }
}
