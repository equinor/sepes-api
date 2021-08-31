using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Automapper
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
            if (VmRuleUtils.InternetIsOpen(source) && source != null && source.ResourceType == AzureResourceType.VirtualMachine)
            {
                return new Dictionary<string, string>() { { "InternetIsOpen", "true" } };
            }

            return null;
        }
    }
}
