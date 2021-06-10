using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class PreApprovedDatasetModelService : DatasetModelServiceBase, IPreApprovedDatasetModelService
    {
        readonly IUserService _userService;

        public PreApprovedDatasetModelService(IConfiguration configuration, SepesDbContext db, ILogger<PreApprovedDatasetModelService> logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, db, logger, studyPermissionService)
        {
            _userService = userService;
        }


        public async Task<Dataset> GetByIdAsync(int datasetId, UserOperation userOperation)
        {
            var queryable = ActiveDatasetsQueryable();

            var dataset = await GetFromQueryableThrowIfNotFoundOrNoAccess(queryable, datasetId, userOperation);
            return dataset;
        }

        public async Task<List<Dataset>> GetAllAsync(UserOperation userOperation)
        {
            await ThrowIfOperationNotAllowed(userOperation);

            var queryable = ActiveDatasetsQueryable();

            return await queryable.ToListAsync();
        }

        public async Task<Dataset> GetForResourceAndFirewall(int datasetId, UserOperation userOperation)
        {
            var queryable = ActiveDatasetsWithResourceAndFirewallQueryable();
            return await GetFromQueryableThrowIfNotFoundOrNoAccess(queryable, datasetId, userOperation);
        }
        public async Task<Dataset> CreateAsync(Dataset newDataset)
        {
            await ThrowIfOperationNotAllowed(UserOperation.PreApprovedDataset_Create_Update_Delete);           
            var addedDataset = await AddAsync(newDataset);
            return await GetByIdWithoutPermissionCheckAsync(addedDataset.Id);
        }

        async Task<Dataset> GetFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Dataset> queryable, int datasetId, UserOperation operation)
        {
            var dataset = await GetFromQueryableThrowIfNotFound(queryable, datasetId);
            await ThrowIfOperationNotAllowed(operation);

            return dataset;
        }

        async Task ThrowIfOperationNotAllowed(UserOperation operation)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            OperationAccessUtil.HasAccessToOperationOrThrow(currentUser, operation);
        }

        IQueryable<Dataset> AddBasicFilters(IQueryable<Dataset> queryable)
        {
            return queryable.Where(ds => !ds.Deleted && !ds.StudySpecific);
        }

        IQueryable<Dataset> ActiveDatasetsQueryable()
        {
            return AddBasicFilters(_db.Datasets.
                Include(ds => ds.StudyDatasets).ThenInclude(sd => sd.Study).ThenInclude(sd => sd.StudyParticipants));
        }

        IQueryable<Dataset> ActiveDatasetsWithResourceAndFirewallQueryable()
        {
            return AddBasicFilters(ActiveDatasetsQueryable().Include(ds => ds.Resources).Include(ds => ds.FirewallRules));
        }
    }
}
