using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
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
                 .Where(ds => ds.Deleted.HasValue == false || ds.Deleted.HasValue && ds.Deleted.Value == false);

            if (excludeStudySpecific)
            {
                queryable = queryable.Where(ds => ds.StudyId.HasValue == false);
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

            ThrowIfOperationNotAllowed(operation);

            return datasetFromDb;
        }

        protected async Task SoftDeleteAsync(Dataset dataset)
        {
            dataset.Deleted = true;
            dataset.DeletedAt = DateTime.UtcNow;
            dataset.DeletedBy = _userService.GetCurrentUser().UserName;
            await _db.SaveChangesAsync();
        }

        protected async Task HardDeleteAsync(Dataset dataset)
        {
            dataset.StudyDatasets.Clear();
            _db.Datasets.Remove(dataset);
            await _db.SaveChangesAsync();
        }

        protected void ThrowIfOperationNotAllowed(UserOperation operation)
        {
            if (StudyAccessUtil.HasAccessToOperation(_userService, operation) == false)
            {
                throw new ForbiddenException($"User {_userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation}");
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
