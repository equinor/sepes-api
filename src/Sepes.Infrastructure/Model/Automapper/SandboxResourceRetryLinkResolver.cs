using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceRetryLinkResolver : IValueResolver<CloudResource, SandboxResourceLight, string>
    {
        public readonly IConfiguration _config;

        public SandboxResourceRetryLinkResolver(IConfiguration config)
        {
            this._config = config;
        }

        public string Resolve(CloudResource source, SandboxResourceLight destination, string destMember, ResolutionContext context)
        {
            if(source != null)
            {
                if (source.ResourceType == AzureResourceType.VirtualMachine)
                {
                    var shouldHaveRetryLink = false;

                    var baseStatusOnThisOperation = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(source);

                    if (baseStatusOnThisOperation == null)
                    {
                        shouldHaveRetryLink = true;
                    }
                    else if (baseStatusOnThisOperation.Status == CloudResourceOperationState.FAILED && baseStatusOnThisOperation.TryCount >= baseStatusOnThisOperation.MaxTryCount)
                    {
                        shouldHaveRetryLink = true;
                    }

                    if (shouldHaveRetryLink)
                    {
                        return AzureResourceUtil.CreateResourceRetryLink(source.Id);
                    }
                }
            }        

            return null;
        }
    }
}
