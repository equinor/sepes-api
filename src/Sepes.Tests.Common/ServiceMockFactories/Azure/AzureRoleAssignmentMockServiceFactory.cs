using System.Collections.Generic;
using Moq;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Threading;
using Sepes.Infrastructure.Dto.Azure.RoleAssignment;

namespace Sepes.Test.Common.ServiceMockFactories
{
    public static class AzureRoleAssignmentMockServiceFactory
    {
        public static Mock<IAzureRoleAssignmentService> CreateBasicForCreate() {

            var mock = new Mock<IAzureRoleAssignmentService>();

            mock.Setup(us => us.AddRoleAssignment(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new AzureRoleAssignment());
            mock.Setup(us => us.DeleteRoleAssignment(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new AzureRoleAssignment());
            
            mock.Setup(us => us.GetResourceGroupRoleAssignments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<AzureRoleAssignment>());
            return mock;
        }
    }
}
