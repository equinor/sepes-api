using AutoMapper;
using Microsoft.Extensions.Configuration;
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
            if (source != null)
            {

                var shouldHaveRetryLink = false;

                var baseStatusOnThisOperation = AzureResourceStatusUtil.DecideWhatOperationToBaseStatusOn(source);

                if (baseStatusOnThisOperation == null)
                {
                    shouldHaveRetryLink = true;
                }
                else if (CloudResourceOperationUtil.HasValidStateForRetry(baseStatusOnThisOperation))
                {
                    shouldHaveRetryLink = true;
                }

                if (shouldHaveRetryLink)
                {
                    return AzureResourceUtil.CreateResourceRetryLink(source.Id);
                }

            }

            return null;
        }
    }
}
