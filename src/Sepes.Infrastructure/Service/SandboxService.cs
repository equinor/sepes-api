using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : ISandboxService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IStudyService _studyService;

        public SandboxService(SepesDbContext db, IMapper mapper, IStudyService studyService)
        {
            _db = db;
            _mapper = mapper;
            _studyService = studyService;
        }

        public async Task<IEnumerable<SandboxDto>> GetSandboxesByStudyIdAsync(int studyId)
        {
            var studyFromDb = await GetStudyOrThrowAsync(studyId);
            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        public async Task<StudyDto> AddSandboxToStudyAsync(int studyId, SandboxDto newSandbox)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await GetStudyOrThrowAsync(studyId);

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(studyFromDb.WbsCode))
            {
                throw new ArgumentException("WBS code missing in Study. Study requires WBS code before sandbox can be created.");
            }
            // TODO: Do check on Sandbox

            // Create reference
            var sandbox = _mapper.Map<Sandbox>(newSandbox);
            studyFromDb.Sandboxes.Add(sandbox);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyByIdAsync(studyId);
        }

        public async Task<StudyDto> RemoveSandboxFromStudyAsync(int studyId, int sandboxId)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await GetStudyOrThrowAsync(studyId);
            var sandboxFromDb = await _db.Sandboxes.FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            // TODO: Do check on Sandbox

            // Create reference
            //var sandbox = _mapper.Map<Sandbox>(sandboxFromDb);
            studyFromDb.Sandboxes.Remove(sandboxFromDb);
            await _db.SaveChangesAsync();

            return await _studyService.GetStudyByIdAsync(studyId);
        }

        async Task<Study> GetStudyOrThrowAsync(int studyId)
        {
            var studyFromDb = await StudyQueries.GetQueryableForStudiesLookup(_db)
                .FirstOrDefaultAsync(s => s.Id == studyId);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Study", studyId);
            }

            return studyFromDb;
        }
    }
}
