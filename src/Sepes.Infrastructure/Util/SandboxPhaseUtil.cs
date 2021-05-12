using Sepes.Common.Model;
using Sepes.Infrastructure.Model;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class SandboxPhaseUtil
    {
        public static SandboxPhase GetNextPhase(Sandbox sandbox)
        {
            var currentPhase = GetCurrentPhase(sandbox);

            var currentPhaseInt = (int)currentPhase;
            var nextPhaseInt = currentPhaseInt + 1;

            if (Enum.IsDefined(typeof(SandboxPhase), nextPhaseInt))
            {
                return (SandboxPhase)(nextPhaseInt);
            }

            throw new Exception($"There is no next phase defined after {currentPhase}-{currentPhase}");
        }      

        public static SandboxPhase GetCurrentPhase(Sandbox sandbox)
        {
            var item = GetCurrentPhaseHistoryItem(sandbox);

            if (item == null)
            {
                return SandboxPhase.Open;
            }

            return item.Phase;
        }

        public static SandboxPhaseHistory GetCurrentPhaseHistoryItem(Sandbox sandbox)
        {
            if (sandbox == null)
            {
                throw new ArgumentNullException("sandbox");
            }

            if (sandbox.PhaseHistory == null)
            {
                throw new Exception("Possibly missing include for Sandbox.PhaseHistory");
            }

            return sandbox.PhaseHistory.OrderByDescending(ph => ph.Counter).FirstOrDefault();
        }
    }
}
