using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<IEnumerable<SandboxDto>> GetSandboxesForStudyAsync(int studyId);
        Task<StudyDto> ValidateSandboxAsync(int studyId, SandboxDto newSandbox);
        Task<SandboxDto> CreateAsync(int studyId, SandboxCreateDto newSandbox);
        Task<SandboxDto> DeleteAsync(int studyId, int sandboxId);

        //Task<IEnumerable<SandboxTemplateDto>> GetTemplatesAsync();
    }
}
