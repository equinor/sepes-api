using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto.Interfaces;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceExternalLinkResolver : IValueResolver<CloudResource, IHasLinkToExtSystem, string>
    {
        public readonly IConfiguration _config;
        public SandboxResourceExternalLinkResolver(IConfiguration config)
        {
            this._config = config;
        }       

        public string Resolve(CloudResource source, IHasLinkToExtSystem destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceLink(_config, source);
        }
    }
   
}
