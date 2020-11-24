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
        readonly IUserService _userService;

        public SandboxDatasetService(SepesDbContext db, IMapper mapper, IUserService userService)
            :base(db, mapper)
        {            

            _userService = userService;
        }

        public async Task<IEnumerable<SandboxDatasetDto>> GetAll(int sandboxId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperations.Study_Crud_Sandbox);

            var datasetsFromDb = await _db.SandboxDatasets
                .Include(sd=> sd.Dataset)
                .Include(s=> s.Sandbox)
                .ThenInclude(sb=> sb.Study)
                .Where(ds => ds.SandboxId == sandboxId)          
                .ToListAsync();   
            
            var dataasetDtos = _mapper.Map<IEnumerable<SandboxDatasetDto>>(datasetsFromDb);

            return dataasetDtos;
        }          

        public async Task<SandboxDatasetDto> Add(int sandboxId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperations.Study_Crud_Sandbox);   
       
            var datasetFromDb = await _db.Datasets.FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if(datasetFromDb.StudyId.HasValue && datasetFromDb.StudyId != studyFromDb.Id)
            {
                throw new ArgumentException($"Dataset {datasetId} cannot be added to Sandbox {sandboxId}. The dataset is Study specific and belongs to another Study than {studyFromDb.Id}.");
            }

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }          

            // Create new linking table
            var sandboxDataset = new SandboxDataset { SandboxId = sandboxId, DatasetId = datasetId, Added = DateTime.UtcNow, AddedBy = _userService.GetCurrentUser().UserName };
            await _db.SandboxDatasets.AddAsync(sandboxDataset);
            await _db.SaveChangesAsync();

            return _mapper.Map<SandboxDatasetDto>(sandboxDataset);
        }     

        public async Task<SandboxDatasetDto> Remove(int sandboxId, int datasetId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyBySandboxIdCheckAccessOrThrow(_db, _userService, sandboxId, UserOperations.Study_Crud_Sandbox);
            var sandboxDataset = await _db.SandboxDatasets
                .FirstOrDefaultAsync(ds => ds.SandboxId == sandboxId && ds.DatasetId == datasetId);

            //Is dataset linked to a study?
            if (sandboxDataset == null)
            {
                throw NotFoundException.CreateForEntity("SandboxDataset", datasetId);
            }
            else
            {
                _db.SandboxDatasets.Remove(sandboxDataset);
                await _db.SaveChangesAsync();
            }

            return _mapper.Map<SandboxDatasetDto>(sandboxDataset);
        }  
    }
}
