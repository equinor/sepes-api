using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceExternalLinkResolver : IValueResolver<SandboxResource, SandboxResourceLightDto, string>
    {
        public readonly IConfiguration _config;
        public SandboxResourceExternalLinkResolver(IConfiguration config)
        {
            this._config = config;
        }       

        public string Resolve(SandboxResource source, SandboxResourceLightDto destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceLink(_config, source);
        }
    }
   
}
