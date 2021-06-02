using AutoMapper;
using Sepes.Common.Interface;
using Sepes.Common.Model;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Automapper
{
    public class SandboxPhaseNameResolver : IValueResolver<Sandbox, IHasCurrentPhase, SandboxPhase>
    {
        public SandboxPhase Resolve(Sandbox source, IHasCurrentPhase destination, SandboxPhase destMember, ResolutionContext context)
        {          
            return SandboxPhaseUtil.GetCurrentPhase(source);
        }
    }
}
