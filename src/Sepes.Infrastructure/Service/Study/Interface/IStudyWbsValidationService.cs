using System.Threading.Tasks;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyWbsValidationService
    {
        Task ValidateForStudyCreateOrUpdate(Study study);
        
        Task ValidateForSandboxCreationOrThrow(Study study);
    }
}