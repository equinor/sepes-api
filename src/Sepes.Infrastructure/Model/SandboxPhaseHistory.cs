using Sepes.Common.Model;

namespace Sepes.Infrastructure.Model
{
    public class SandboxPhaseHistory : BaseModel
    {
        public int SandboxId { get; set; }

        public int Counter { get; set; }

        public SandboxPhase Phase { get; set; }

        public Sandbox Sandbox { get; set; }
    }
}
