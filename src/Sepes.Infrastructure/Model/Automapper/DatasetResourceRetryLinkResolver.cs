using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class DatasetResourceRetryLinkResolver : IValueResolver<CloudResource, DatasetResourceLightDto, string>
    {
        public readonly IConfiguration _config;

        public DatasetResourceRetryLinkResolver(IConfiguration config)
        {
            this._config = config;
        }       

        public string Resolve(CloudResource source, DatasetResourceLightDto destination, string destMember, ResolutionContext context)
        {            
            var baseStatusOnThisOperation = ResourceStatusUtil.DecideWhatOperationToBaseStatusOn(source);

            if(baseStatusOnThisOperation != null)
            {
                if (source.ResourceType == AzureResourceType.StorageAccount
                             && source.Purpose == CloudResourcePurpose.StudySpecificDatasetStorageAccount
                             && baseStatusOnThisOperation.Status == CloudResourceOperationState.FAILED
                             && baseStatusOnThisOperation.TryCount >= baseStatusOnThisOperation.MaxTryCount)
                {
                    return AzureResourceUtil.CreateResourceRetryLink(source.Id);
                }
            }         

            return null;          
        }
    }   
}
