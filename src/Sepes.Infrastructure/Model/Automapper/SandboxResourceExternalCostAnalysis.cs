using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Common.Util;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceExternalCostAnalysis : IValueResolver<Sandbox, SandboxDetails, string>
    {
        public readonly IConfiguration _config;
        public SandboxResourceExternalCostAnalysis(IConfiguration config)
        {
            this._config = config;
        }

        public string Resolve(Sandbox source, SandboxDetails destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceCostLink(_config, source);
        }
    }
}
