using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Automapper
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
            return CloudResourceUtil.CreateResourceCostLink(_config, source);
        }
    }
}
