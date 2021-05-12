using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Response.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : SandboxServiceBase, ISandboxService
    {
        readonly IStudyModelService _studyModelService;
        readonly ISandboxResourceCreateService _sandboxResourceCreateService;
        readonly ISandboxResourceDeleteService _sandboxResourceDeleteService;

        public SandboxService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService, IStudyModelService studyModelService, ISandboxModelService sandboxModelService, ISandboxResourceCreateService sandboxCloudResourceService, ISandboxResourceDeleteService sandboxResourceDeleteService)
            : base(config, db, mapper, logger, userService, sandboxModelService)
        {
            _studyModelService = studyModelService;
            _sandboxResourceCreateService = sandboxCloudResourceService;
            _sandboxResourceDeleteService = sandboxResourceDeleteService;
        }       

        public async Task<SandboxDetails> GetSandboxDetailsAsync(int sandboxId)
        {
            return await GetSandboxDetailsInternalAsync(sandboxId);
        }             

        public async Task<SandboxDetails> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {
            _logger.LogInformation(SepesEventId.SandboxCreate, "Sandbox {0}: Starting", studyId);

            Sandbox createdSandbox = null;

            GenericNameValidation.ValidateName(sandboxCreateDto.Name);          

            if (String.IsNullOrWhiteSpace(sandboxCreateDto.Region))
            {
                throw new ArgumentException("Region not specified.");
            }

            // Verify that study with that id exists
            var study = await _studyModelService.GetForSandboxCreateAndDeleteAsync(studyId, UserOperation.Study_Crud_Sandbox);

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(study.WbsCode))
            {
                throw new ArgumentException("WBS code missing in Study. Study requires WBS code before Sandbox can be created.");
            }

            //Check uniqueness of name
            if (await _db.Sandboxes.Where(sb => sb.StudyId == studyId && sb.Name == sandboxCreateDto.Name && !sb.Deleted).AnyAsync())
            {
                throw new ArgumentException($"A Sandbox called {sandboxCreateDto.Name} allready exists for Study");
            }

            try
            {
                var user = await _userService.GetCurrentUserAsync();

                createdSandbox = _mapper.Map<Sandbox>(sandboxCreateDto);

                InitiatePhaseHistory(createdSandbox, user);

                createdSandbox.CreatedBy = user.UserName;
                createdSandbox.TechnicalContactName = user.FullName;
                createdSandbox.TechnicalContactEmail = user.EmailAddress;

                study.Sandboxes.Add(createdSandbox);

                await _db.SaveChangesAsync();

                try
                { 
                    await _sandboxResourceCreateService.CreateBasicSandboxResourcesAsync(createdSandbox);
                }
                catch (Exception)
                {
                    //Deleting sandbox entry and all related from DB
                    if (createdSandbox.Id > 0)
                    {
                        foreach (var curRes in await _db.CloudResources.Include(r => r.Operations).Where(r => r.SandboxId == createdSandbox.Id).ToListAsync())
                        {
                            foreach (var curOp in curRes.Operations)
                            {
                                _db.CloudResourceOperations.Remove(curOp);
                            }

                            _db.CloudResources.Remove(curRes);
                        }

                        study.Sandboxes.Remove(createdSandbox);
                        await _db.SaveChangesAsync();
                    }

                    throw;
                }

                return await GetSandboxDetailsAsync(createdSandbox.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Sandbox creation failed: {ex.Message}", ex);
            }
        }     

        public async Task DeleteAsync(int sandboxId)
        {
            _logger.LogWarning(SepesEventId.SandboxDelete, "Sandbox {0}: Starting", sandboxId);

            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);

            int studyId = sandboxFromDb.StudyId;

            var user = await _userService.GetCurrentUserAsync();

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking sandbox record for deletion", studyId, sandboxId);

            SoftDeleteUtil.MarkAsDeleted(sandboxFromDb, user);

            await _db.SaveChangesAsync();

            await _sandboxResourceDeleteService.HandleSandboxDeleteAsync(sandboxId);

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Done", studyId, sandboxId);
        }
    }
}
