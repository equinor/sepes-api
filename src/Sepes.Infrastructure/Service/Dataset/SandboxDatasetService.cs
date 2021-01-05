using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxDatasetService : ServiceBase<Dataset>, ISandboxDatasetService
    {

        public SandboxDatasetService(SepesDbContext db, IMapper mapper, IUserService userService)
            :base(db, mapper, userService)
        {            


        }

        public async Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperation.Study_Read);

            var datasetsFromDb = await _db.SandboxDatasets
                .Include(sd=> sd.Dataset)
                .Include(s=> s.Sandbox)
                .ThenInclude(sb=> sb.Study)
                .Where(ds => ds.SandboxId == sandboxId)          
                .ToListAsync();   
            
            var dataasetDtos = _mapper.Map<IEnumerable<SandboxDatasetDto>>(datasetsFromDb);

            return dataasetDtos;
        }   
        
      

        public async Task Add(int sandboxId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperation.Study_Crud_Sandbox);

            await ValidateAddOrRemoveDataset(sandboxId);

            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            if (datasetFromDb.StudyId.HasValue && datasetFromDb.StudyId != studyFromDb.Id)
            {
                throw new ArgumentException($"Dataset {datasetId} cannot be added to Sandbox {sandboxId}. The dataset is Study specific and belongs to another Study than {studyFromDb.Id}.");
            }

            var sandboxDatasetRelation = await _db.SandboxDatasets.FirstOrDefaultAsync(ds => ds.SandboxId == sandboxId && ds.DatasetId == datasetId);

            //Is dataset allready linked to this sandbox?
            if (sandboxDatasetRelation != null)
            {
                throw new ArgumentException($"Dataset is allready added to Sandbox.");
            }

            // Create new entry in the relation table
            var sandboxDataset = new SandboxDataset { SandboxId = sandboxId, DatasetId = datasetId, Added = DateTime.UtcNow, AddedBy = (await _userService.GetCurrentUserAsync()).UserName };
            await _db.SandboxDatasets.AddAsync(sandboxDataset);
            await _db.SaveChangesAsync();
        }     

        public async Task Remove(int sandboxId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperation.Study_Crud_Sandbox);

            await ValidateAddOrRemoveDataset(sandboxId);

            var sandboxDatasetRelation = await _db.SandboxDatasets.FirstOrDefaultAsync(ds => ds.SandboxId == sandboxId && ds.DatasetId == datasetId);

            //Is dataset actually linked to this sandbox?
            if (sandboxDatasetRelation == null)
            {
                throw new ArgumentException($"Dataset could not be removed from Sandbox, as it is not associated with it.");
            }
            else
            {
                _db.SandboxDatasets.Remove(sandboxDatasetRelation);
                await _db.SaveChangesAsync();
            }           
        }

        async Task ValidateAddOrRemoveDataset(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxByIdNoChecksAsync(sandboxId);

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            var sandboxPhase = SandboxPhaseUtil.GetCurrentPhase(sandboxFromDb);

            if (sandboxPhase > SandboxPhase.Open)
            {
                throw new ArgumentException($"Dataset cannot be added to Sandbox. Sandbox phase must be open.");
            }
        }
    }
}
