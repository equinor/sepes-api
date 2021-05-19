//using Microsoft.Extensions.Logging;
//using Sepes.Azure.Service.Interface;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Sepes.Tests.Common.Mocks.Azure
//{
//    public class AzureResourceGroupServiceMock : AzureGenericProvisioningServiceMock, IAzureResourceGroupService
//    {
//        public AzureResourceGroupServiceMock(ILogger<AzureGenericProvisioningServiceMock> logger, string resourceType) 
//            :base(logger, resourceType)
//        {
          
//        }         

//        public Task Delete(string resourceGroupName, CancellationToken cancellationToken = default)
//        {
//            return Task.CompletedTask;
//        }      
//    }
//}
