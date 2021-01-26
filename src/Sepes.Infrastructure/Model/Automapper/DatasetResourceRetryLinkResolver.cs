using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.Dataset;
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
            var baseStatusOnThisOperation = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(source);

            if (source.ResourceType == AzureResourceType.StorageAccount
                && source.Purpose == CloudResourcePurpose.StudySpecificDatasetStorageAccount
                && baseStatusOnThisOperation.Status == CloudResourceOperationState.FAILED
                && baseStatusOnThisOperation.TryCount >= baseStatusOnThisOperation.MaxTryCount)
            {
                return AzureResourceUtil.CreateResourceRetryLink(source.Id);
            }

            return null;          
        }
    }   
}
