//using Microsoft.Extensions.Logging;
//using Sepes.Common.Dto;
//using Sepes.Azure.Service.Interface;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Sepes.Tests.Common.Mocks.Azure
//{
//    public class AzureRoleAssignmentServiceMock : AzureGenericProvisioningServiceMock, IAzureRoleAssignmentService
//    {
//        public AzureRoleAssignmentServiceMock(ILogger<AzureRoleAssignmentServiceMock> logger, string resourceType) 
//            :base(logger, resourceType)
//        {
          
//        }       

//        public Task SetRoleAssignments(string resourceGroupId, string resourceGroupName, List<CloudResourceDesiredRoleAssignmentDto> desiredRoleAssignments, CancellationToken cancellationToken = default)
//        {
//            return Task.CompletedTask;
//        }
//    }
//}
