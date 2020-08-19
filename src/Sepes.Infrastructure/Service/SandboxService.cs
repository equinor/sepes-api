﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : ISandboxService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IStudyService _studyService;
        readonly ISandboxWorkerService _sandboxWorkerService;

        public SandboxService(SepesDbContext db, IMapper mapper, IStudyService studyService, ISandboxWorkerService sandboxWorkerService)
        {
            _db = db;
            _mapper = mapper;
            _studyService = studyService;
            _sandboxWorkerService = sandboxWorkerService;
        }

        public async Task<IEnumerable<SandboxDto>> GetSandboxesForStudyAsync(int studyId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        async Task<Sandbox> GetSandboxOrThrowAsync(int sandboxId)
        {
            var sandboxFromDb = await _db.Sandboxes
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Sandbox", sandboxId);
            }
            return sandboxFromDb;
        }

        async Task<SandboxDto> GetSandboxDtoAsync(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);
           return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

            // TODO Validate azure things
            public async Task<StudyDto> ValidateSandboxAsync(int studyId, SandboxDto newSandbox)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            return await ValidateSandboxAsync(studyFromDb, newSandbox);
      
        }

        Task<StudyDto> ValidateSandboxAsync(Study study, SandboxDto newSandbox)
        {
            throw new NotImplementedException();
        }

        public async Task<SandboxDto> CreateAsync(int studyId, SandboxCreateDto newSandbox)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(studyFromDb.WbsCode))
            {
                throw new ArgumentException("WBS code missing in Study. Study requires WBS code before sandbox can be created.");
            }
            // TODO: Do check on Sandbox

           
            var sandbox = _mapper.Map<Sandbox>(newSandbox);
            studyFromDb.Sandboxes.Add(sandbox);
            await _db.SaveChangesAsync();
        
            // Get Dtos for arguments to sandboxWorkerService
            var studyDto = await _studyService.GetStudyByIdAsync(studyId);          
            var sandboxDto = await GetSandboxDtoAsync(sandbox.Id);
           
            var tags = AzureResourceTagsFactory.CreateTags(studyFromDb.Name, studyDto, sandboxDto);
          
            var region = RegionStringConverter.Convert(newSandbox.Region);
            await _sandboxWorkerService.CreateBasicSandboxResourcesAsync(sandbox.Id, region, studyFromDb.Name, tags);
       
            return await GetSandboxDtoAsync(sandbox.Id);
        }
    
        // TODO: DO stuff inn azure
        public async Task<SandboxDto> DeleteAsync(int studyId, int sandboxId)
        {
            // Run validations: (Check if ID is valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var sandboxFromDb = await _db.Sandboxes.FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Sandbox", sandboxId);
            }

            studyFromDb.Sandboxes.Remove(sandboxFromDb);
            await _db.SaveChangesAsync();
            return _mapper.Map<SandboxDto>(sandboxFromDb);
    
        }

      
    }
}
