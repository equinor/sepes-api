using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceExternalCostAnalysis : IValueResolver<Sandbox, SandboxDetailsDto, string>
    {
        public readonly IConfiguration _config;
        public SandboxResourceExternalCostAnalysis(IConfiguration config)
        {
            this._config = config;
        }

        public string Resolve(Sandbox source, SandboxDetailsDto destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceCostLink(_config, source);
        }
    }
}
