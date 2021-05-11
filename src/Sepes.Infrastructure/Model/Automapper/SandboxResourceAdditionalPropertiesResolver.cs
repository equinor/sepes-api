using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Common.Util;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceAdditionalPropertiesResolver : IValueResolver<CloudResource, SandboxResourceLight, Dictionary<string, string>>
    {
        public readonly IConfiguration _config;

        public SandboxResourceAdditionalPropertiesResolver(IConfiguration config)
        {
            this._config = config;
        }

        public Dictionary<string, string> Resolve(CloudResource source, SandboxResourceLight destination, Dictionary<string, string> destMember, ResolutionContext context)
        {
            if(source != null)
            {
                if (source.ResourceType == AzureResourceType.VirtualMachine)
                {
                    if (AzureVmUtil.InternetIsOpen(source)) {
                        return new Dictionary<string, string>() { { "InternetIsOpen", "true" } };
                    } 
                }
            }        

            return null;
        }
    }
}
