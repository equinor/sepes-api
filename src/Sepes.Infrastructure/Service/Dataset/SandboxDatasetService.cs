using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Service.DataModelService.Interface;
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
        ISandboxModelService _sandboxModelService;

        public SandboxDatasetService(SepesDbContext db, IMapper mapper, IUserService userService, ISandboxModelService sandboxModelService)
            : base(db, mapper, userService)
        {
            _sandboxModelService = sandboxModelService;
        }

        public async Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperation.Study_Read);

            var datasetsFromDb = await _db.SandboxDatasets
                .Include(sd => sd.Dataset)
                .Include(s => s.Sandbox)
                .ThenInclude(sb => sb.Study)
                .Where(ds => ds.SandboxId == sandboxId)
                .ToListAsync();

            var datasetDtos = _mapper.Map<IEnumerable<SandboxDatasetDto>>(datasetsFromDb);

            return datasetDtos;
        }      

        public async Task<AvailableDatasets> AllAvailable(int sandboxId)
        {
            var sandbox = await _sandboxModelService.GetSandboxForDatasetOperationsAsync(sandboxId, UserOperation.Study_Read, true, false);

            return MapToAvailable(sandbox);          
        }

        AvailableDatasets MapToAvailable(Sandbox sandbox)
        { 
            var availableDatasets = sandbox.Study
                .StudyDatasets
                .Select(sd => new AvailableDatasetItem()
                {
                    DatasetId = sd.DatasetId,
                    Name = sd.Dataset.Name,
                    Classification = sd.Dataset.Classification,
                    AddedToSandbox = sd.Dataset.SandboxDatasets.Where(sd => sd.SandboxId == sandbox.Id).Any()
                });

            var result = new AvailableDatasets(availableDatasets);
            DatasetClassificationUtils.SetRestrictionProperties(result);

            return result;
        }

        public async Task<AvailableDatasets> Add(int sandboxId, int datasetId)
        {
            var sandbox = await _sandboxModelService.GetSandboxForDatasetOperationsAsync(sandboxId, UserOperation.Study_Crud_Sandbox, false, true);

            ValidateAddOrRemoveDataset(sandbox);

            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            if (datasetFromDb.StudySpecific)
            {
                var studyForDataset = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(datasetFromDb);


                if (datasetFromDb.StudySpecific && studyForDataset.Id != sandbox.Study.Id)
                {
                    throw new ArgumentException($"Dataset {datasetId} cannot be added to Sandbox {sandboxId}. The dataset is Study specific and belongs to another Study than {sandbox.Study.Id}.");
                }
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

            return MapToAvailable(sandbox);
        }

        public async Task<AvailableDatasets> Remove(int sandboxId, int datasetId)
        {
            var sandbox = await _sandboxModelService.GetSandboxForDatasetOperationsAsync(sandboxId, UserOperation.Study_Crud_Sandbox, false, true);

            ValidateAddOrRemoveDataset(sandbox);

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

            return MapToAvailable(sandbox);
        }

        void ValidateAddOrRemoveDataset(Sandbox sandbox)
        { 
            var sandboxPhase = SandboxPhaseUtil.GetCurrentPhase(sandbox);

            if (sandboxPhase > SandboxPhase.Open)
            {
                throw new ArgumentException($"Dataset cannot be added to Sandbox. Sandbox phase must be open.");
            }
        }


    }
}
