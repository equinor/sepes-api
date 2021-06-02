using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudySpecificDatasetModelService : DatasetModelServiceBase, IStudySpecificDatasetModelService
    {
        public StudySpecificDatasetModelService(IConfiguration configuration, SepesDbContext db, ILogger<StudySpecificDatasetModelService> logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, db, logger, userService, studyPermissionService)
        {

        }

        public async Task<Dataset> GetByIdWithResourceAndFirewallWithoutPermissionCheckAsync(int datasetId)
        {
            return await ActiveDatasetsWithResourceAndFirewallQuerayble().FirstOrDefaultAsync(r=> r.Id == datasetId);
        }      

        public async Task<Dataset> GetByIdAsync(int datasetId, UserOperation userOperation)
        {
            var queryable = ActiveDatasetsQuerayble();

            var dataset = await GetFromQueryableThrowIfNotFoundOrNoAccess(queryable, datasetId, userOperation);
            return dataset;
        }

        public async Task<Dataset> GetForResourceAndFirewall(int datasetId, UserOperation userOperation)
        {
            var queryable = ActiveDatasetsWithResourceAndFirewallQuerayble();
            return await GetFromQueryableThrowIfNotFoundOrNoAccess(queryable, datasetId, userOperation);
        }       

        async Task<Dataset> GetFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Dataset> queryable, int datasetId, UserOperation operation)
        {
            var dataset = await GetFromQueryableThrowIfNotFound(queryable, datasetId);

            var study = DatasetUtils.GetStudyFromStudySpecificDatasetOrThrow(dataset);

            await CheckAccesAndThrowIfNotAllowed(study, operation);

            return dataset;
        }

        IQueryable<Dataset> ActiveDatasetsQuerayble()
        {
            return _db.Datasets.
                Include(ds => ds.StudyDatasets).ThenInclude(sd => sd.Study).ThenInclude(sd => sd.StudyParticipants)
             .Where(ds => !ds.Deleted);
        }

        IQueryable<Dataset> ActiveDatasetsWithResourceAndFirewallQuerayble()
        {
            return ActiveDatasetsQuerayble().Include(ds => ds.Resources).ThenInclude(r=> r.Operations).Include(ds => ds.FirewallRules);
        }
    }
}
