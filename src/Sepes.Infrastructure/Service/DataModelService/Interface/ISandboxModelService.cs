﻿using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ISandboxModelService
    {
        Task<Sandbox> AddAsync(Study study, Sandbox sandbox);  

        Task<Sandbox> GetByIdAsync(int id, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false);

        Task<Sandbox> GetByIdForResourcesAsync(int id);

        Task<Sandbox> GetByIdForCostAnalysisLinkAsync(int id, UserOperation userOperation);

        Task<Sandbox> GetDetailsByIdAsync(int id);

        Task<string> GetRegionByIdAsync(int id, UserOperation userOperation);

        Task<Sandbox> GetWithResourcesNoPermissionCheckAsync(int id);
        Task<Sandbox> GetByIdForPhaseShiftAsync(int id, UserOperation userOperation);
        Task<Sandbox> GetByIdForReScheduleCreateAsync(int sandboxId);
        Task<Sandbox> GetByIdForResourceCreationAsync(int id, UserOperation userOperation);
        Task<bool> NameIsTaken(int studyId, string sandboxName);
        Task HardDeleteAsync(int sandboxId);
        Task SoftDeleteAsync(int sandboxId);
    }
}
