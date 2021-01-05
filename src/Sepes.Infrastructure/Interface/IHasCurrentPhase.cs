using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Interface
{
    public interface IHasCurrentPhase
    {
        SandboxPhase CurrentPhase { get; set; }
    }
}
