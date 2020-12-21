using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceRetryLinkResolver : IValueResolver<CloudResource, SandboxResourceLightDto, string>
    {
        public readonly IConfiguration _config;
        public SandboxResourceRetryLinkResolver(IConfiguration config)
        {
            this._config = config;
        }       

        public string Resolve(CloudResource source, SandboxResourceLightDto destination, string destMember, ResolutionContext context)
        {            
            var baseStatusOnThisOperation = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(source);

            if (source.ResourceType == AzureResourceType.VirtualMachine && baseStatusOnThisOperation.Status == CloudResourceOperationState.FAILED && baseStatusOnThisOperation.TryCount >= baseStatusOnThisOperation.MaxTryCount)
            {
                return AzureResourceUtil.CreateResourceRetryLink(source.Id);
            }

            return null;          
        }
    }   
}
