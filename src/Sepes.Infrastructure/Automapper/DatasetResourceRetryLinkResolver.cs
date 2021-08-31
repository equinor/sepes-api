using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto.Dataset;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Automapper
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

            if (source.ResourceType == AzureResourceType.StorageAccount
                             && source.Purpose == CloudResourcePurpose.StudySpecificDatasetStorageAccount
                             && baseStatusOnThisOperation.Status == CloudResourceOperationState.FAILED
                             && baseStatusOnThisOperation.TryCount >= baseStatusOnThisOperation.MaxTryCount
                             && baseStatusOnThisOperation != null)
            {
                return AzureResourceUtil.CreateResourceRetryLink(source.Id);
            }

            return null;          
        }
    }   
}
