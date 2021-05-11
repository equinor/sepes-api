using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Provisioning;

namespace Sepes.Infrastructure.Util
{
    public static class ResourceProvisioningResultUtil
    {
        public static ResourceProvisioningResult CreateFromIResource(IResource resource)
        {
            return new ResourceProvisioningResult() { IdInTargetSystem = resource.Id, NameInTargetSystem = resource.Name };
        }

        public static ResourceProvisioningResult CreateFromIResource(Resource resource)
        {
            return new ResourceProvisioningResult() { IdInTargetSystem = resource.Id, NameInTargetSystem = resource.Name };
        }

        public static ResourceProvisioningResult CreateFromProvisioningState(string provisioningState = null)
        {
            return new ResourceProvisioningResult() { CurrentProvisioningState = provisioningState };
        }
    }
}
