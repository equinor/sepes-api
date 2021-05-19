using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;
using System.Threading.Tasks;

namespace Sepes.Common.Util.Provisioning
{
    public static class ProvisioningParamaterUtil
    {
        public static void PrepareForNewOperation(ResourceProvisioningParameters currentCrudInput, CloudResourceOperationDto currentOperation, ResourceProvisioningResult lastResult, string nsgName = null)
        {  
            currentCrudInput.ResetButKeepSharedVariables(lastResult?.NewSharedVariables);

            currentCrudInput.Name = currentOperation.Resource.ResourceName;
            currentCrudInput.StudyName = currentOperation.Resource.StudyName;
            currentCrudInput.DatabaseId = currentOperation.Resource.Id;
            currentCrudInput.StudyId = currentOperation.Resource.StudyId;
            currentCrudInput.SandboxId = currentOperation.Resource.SandboxId;
            currentCrudInput.DatasetId = currentOperation.Resource.DatasetId;
            currentCrudInput.SandboxName = currentOperation.Resource.SandboxName;
            currentCrudInput.ResourceGroupName = currentOperation.Resource.ResourceGroupName;
            currentCrudInput.Region = currentOperation.Resource.Region;         
            currentCrudInput.Tags = currentOperation.Resource.Tags;
            currentCrudInput.ConfigurationString = currentOperation.Resource.ConfigString;
            currentCrudInput.NetworkSecurityGroupName = nsgName;            
        }
    }
}
