
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class ProvisioningParamaterUtil
    {
        public static async Task PrepareForNewOperation(ResourceProvisioningParameters currentCrudInput, CloudResourceOperationDto currentOperation, ResourceProvisioningResult lastResult, ICloudResourceReadService resourceReadService)
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
            currentCrudInput.Region = RegionStringConverter.Convert(currentOperation.Resource.Region);         
            currentCrudInput.Tags = currentOperation.Resource.Tags;
            currentCrudInput.ConfigurationString = currentOperation.Resource.ConfigString;


            //Only relevant for Sandbox Resource Creation
            if (currentOperation.Resource.SandboxId.HasValue)
            {
                var nsg = CloudResourceUtil.GetSibilingResource(await resourceReadService.GetByIdAsync(currentOperation.Resource.Id), AzureResourceType.NetworkSecurityGroup);
                currentCrudInput.NetworkSecurityGroupName = nsg?.ResourceName;
            }        
        }
    }
}
