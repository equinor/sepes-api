using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Util
{
    public static class ProvisioningQueueUtil
    {
        public static void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResourceOperationDto operation)
        {
            parent.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });
        }

        public static void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResourceOperation operation)
        {
            parent.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });
        }

        public static void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResource resource)
        {
            var createOperation = CloudResourceOperationUtil.GetCreateOperation(resource);
            CreateChildAndAdd(parent, createOperation);
        }
    }
}
