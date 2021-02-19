using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Provisioning;

namespace Sepes.Infrastructure.Util
{
    public static class ResourceProvisioningResultUtil
    {
        public static ResourceProvisioningResult CreateResultFromIResource(IResource resource)
        {
            return new ResourceProvisioningResult() { IdInTargetSystem = resource.Id, NameInTargetSystem = resource.Name };
        }

        public static ResourceProvisioningResult CreateResultFromIResource(Resource resource)
        {
            return new ResourceProvisioningResult() { IdInTargetSystem = resource.Id, NameInTargetSystem = resource.Name };
        }

        public static ResourceProvisioningResult CreateResultFromProvisioningState(string provisioningState)
        {
            return new ResourceProvisioningResult() { CurrentProvisioningState = provisioningState };
        }
    }
}
