using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Common.Mocks.Azure
{
    public class AzureResourceGroupServiceMock : AzureGenericProvisioningServiceMock, IAzureResourceGroupService
    {
        public AzureResourceGroupServiceMock(ILogger<AzureGenericProvisioningServiceMock> logger, string resourceType) 
            :base(logger, resourceType)
        {
          
        }       
       

        public Task Delete(string resourceGroupName, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }      
    }
}
