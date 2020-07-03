using Sepes.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<IEnumerable<SandboxDto>> GetSandboxesByStudyIdAsync(int studyId);
        Task<StudyDto> AddSandboxToStudyAsync(int studyId, SandboxDto newSandbox);
        Task<StudyDto> RemoveSandboxFromStudyAsync(int studyId, int sandboxId);
    }
}
