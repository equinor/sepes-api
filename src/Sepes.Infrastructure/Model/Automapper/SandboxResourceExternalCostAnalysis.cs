using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto.Interfaces;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Model.Automapper
{
    public class SandboxResourceExternalCostAnalysis : IValueResolver<Sandbox, SandboxDto, string>
    {
        public readonly IConfiguration _config;
        public SandboxResourceExternalCostAnalysis(IConfiguration config)
        {
            this._config = config;
        }

        public string Resolve(Sandbox source, SandboxDto destination, string destMember, ResolutionContext context)
        {
            return AzureResourceUtil.CreateResourceCostLink(_config, source);
        }
    }
}
