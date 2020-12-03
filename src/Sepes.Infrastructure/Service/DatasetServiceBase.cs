using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
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

        protected IQueryable<Dataset> DatasetBaseQueryable(bool removeStudySpecific, int datasetId = 0)
        {
            var queryable = _db.Datasets
                 .Include(s => s.StudyDatasets)
                 .ThenInclude(sd => sd.Study).AsQueryable();

            if(datasetId > 0)
            {
                queryable = queryable.Where(ds => ds.Id == datasetId);
            }

            if (removeStudySpecific)
            {
                queryable = queryable.Where(ds => ds.StudyId == null);
            }

            return queryable;
        }

       
        protected async Task<Dataset> GetDatasetOrThrowAsync(int id, UserOperation operation)
        {
            var datasetFromDb = await DatasetBaseQueryable(true, id).FirstOrDefaultAsync();

            if (datasetFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", id);
            }

            ThrowIfOperationNotAllowed(operation);

            return datasetFromDb;
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
    }
}
