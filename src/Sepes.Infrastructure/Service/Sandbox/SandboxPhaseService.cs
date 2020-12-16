using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxPhaseService : SandboxServiceBase, ISandboxPhaseService
    {
        readonly IRequestIdService _requestIdService;
        readonly IStudyService _studyService;
        readonly ISandboxCloudResourceService _sandboxCloudResourceService;     


        public SandboxPhaseService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService,
            IRequestIdService requestIdService, IStudyService studyService, ISandboxCloudResourceService sandboxCloudResourceService)
            :base (config, db, mapper, logger, userService)
        {
        
            _requestIdService = requestIdService;
            _studyService = studyService;       
            _sandboxCloudResourceService = sandboxCloudResourceService;
        }       

        public async Task MoveToNextPhaseAsync(int sandboxId, CancellationToken cancellation = default)
        {
            _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Starting", sandboxId);

            try
            {
                var user = await _userService.GetCurrentUserAsync();

                var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.SandboxLock, true);

                var currentPhaseItem = SandboxPhaseUtil.GetCurrentPhaseHistoryItem(sandboxFromDb);

                var nextPhase = SandboxPhaseUtil.GetNextPhase(sandboxFromDb);

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Moving from {1} to {2}", sandboxId, currentPhaseItem.Phase, nextPhase);

                sandboxFromDb.PhaseHistory.Add(new SandboxPhaseHistory() { Counter = currentPhaseItem.Counter + 1, Phase = nextPhase, CreatedBy = user.UserName });
                await _db.SaveChangesAsync();
                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Phase added to db. Proceeding to make data available", sandboxId);

                //Make data available
                //Connect storage account to vnet              


                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Done", sandboxId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Moving to next phase failed", ex);
            }
        }

        
    }
}
