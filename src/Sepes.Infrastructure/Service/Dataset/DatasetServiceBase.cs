using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class DatasetServiceBase : ServiceBase<Dataset>
    {
        protected readonly ILogger _logger;

        public DatasetServiceBase(SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService)
            : base(db, mapper, userService)
        {
            _logger = logger;
        }

        protected IQueryable<Dataset> DatasetBaseQueryable(bool excludeStudySpecific = true)
        {
            var queryable = _db.Datasets
                 .Include(ds => ds.StudyDatasets)
                 .ThenInclude(sd => sd.Study)
                 .ThenInclude(s=> s.Resources)
                 .ThenInclude(r=> r.Operations)
                 .Include(d=> d.FirewallRules)
                 .Include(d=> d.Resources)
                 .ThenInclude(r=> r.Operations)
                 .Where(ds => !ds.Deleted);

            if (excludeStudySpecific)
            {
                queryable = queryable.Where(ds => !ds.StudyId.HasValue);
            }

            return queryable;
        }

        protected async Task<Dataset> GetDatasetOrThrowNoAccessCheckAsync(int datasetId)
        {
            var datasetFromDb = await DatasetBaseQueryable(false).FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            return datasetFromDb;
        }

        protected async Task<Dataset> GetDatasetOrThrowAsync(int datasetId, UserOperation operation, bool excludeStudySpecific = true)
        {
            var datasetFromDb = await DatasetBaseQueryable(excludeStudySpecific).FirstOrDefaultAsync(ds => ds.Id == datasetId);

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }

            if(datasetFromDb.StudyId.HasValue && datasetFromDb.StudyId.Value > 0)
            {
                await ThrowIfOperationNotAllowed(operation, datasetFromDb.Study);
            }
            else
            {
                await ThrowIfOperationNotAllowed(operation);
            }           

            return datasetFromDb;
        }

        protected async Task SoftDeleteAsync(Dataset dataset)
        {
            await SoftDeleteUtil.MarkAsDeleted(dataset, _userService);           
            await _db.SaveChangesAsync();
        }

        protected async Task HardDeleteAsync(Dataset dataset)
        {
            dataset.StudyDatasets.Clear();
            _db.Datasets.Remove(dataset);
            await _db.SaveChangesAsync();
        }

        protected async Task ThrowIfOperationNotAllowed(UserOperation operation)
        {
            if (!StudyAccessUtil.HasAccessToOperation(await _userService.GetCurrentUserWithStudyParticipantsAsync(), operation))
            {
                throw new ForbiddenException($"User {(await _userService.GetCurrentUserAsync()).EmailAddress} does not have permission to perform operation {operation}");
            }
        }

        protected async Task ThrowIfOperationNotAllowed(UserOperation operation, Study study)
        {
            if (!(await StudyAccessUtil.HasAccessToOperationForStudyAsync(_userService, study, operation)))
            {
                throw new ForbiddenException($"User {(await _userService.GetCurrentUserAsync()).EmailAddress} does not have permission to perform operation {operation}");
            }
        }

        protected void PerformUsualTestForPostedDatasets(DatasetDto datasetDto)
        {
            if (String.IsNullOrWhiteSpace(datasetDto.Name))
            {
                throw new ArgumentException($"Field Dataset.Name is required. Current value: {datasetDto.Name}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Classification))
            {
                throw new ArgumentException($"Field Dataset.Classification is required. Current value: {datasetDto.Classification}");
            }
            if (String.IsNullOrWhiteSpace(datasetDto.Location))
            {
                throw new ArgumentException($"Field Dataset.Location is required. Current value: {datasetDto.Location}");
            }
        }

        protected bool IsStudySpecific(Dataset dataset)
        {
            return dataset.StudyId.HasValue && dataset.StudyId.Value > 0;
        }
    }
}
