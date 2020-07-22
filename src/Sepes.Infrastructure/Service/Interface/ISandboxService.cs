using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxService
    {
        Task<IEnumerable<SandboxDto>> GetSandboxesByStudyIdAsync(int studyId);
        Task<StudyDto> ValidateSandboxAsync(int studyId, SandboxDto newSandbox);
        Task<StudyDto> AddSandboxToStudyAsync(int studyId, SandboxDto newSandbox);
        Task<StudyDto> RemoveSandboxFromStudyAsync(int studyId, int sandboxId);
    }
}
