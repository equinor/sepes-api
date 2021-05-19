using Sepes.Common.Model;

namespace Sepes.Common.Interface
{
    public interface IHasCurrentPhase
    {
        SandboxPhase CurrentPhase { get; set; }
    }
}
