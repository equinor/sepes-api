using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class DatasetModelServiceBase : ModelServiceBase<Dataset>
    {
        public DatasetModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger, IUserService userService)
            : base(configuration, db, logger, userService)
        {

        }

        public async Task<Dataset> GetByIdWithoutPermissionCheckAsync(int datasetId)
        {
            return await _db.Datasets                   
                 .Where(ds => !ds.Deleted && ds.Id == datasetId).FirstOrDefaultAsync();
        }      

        public async Task<bool> IsStudySpecific(int datasetId)
        {
           return await _db.Datasets.Where(ds => ds.Id == datasetId).Select(ds => ds.StudySpecific).FirstOrDefaultAsync();         
        }

        protected async Task<Dataset> GetFromQueryableThrowIfNotFound(IQueryable<Dataset> queryable, int datasetId)
        {
            var dataset = await queryable.SingleOrDefaultAsync(s => s.Id == datasetId);

            if (dataset == null)
            {
                throw NotFoundException.CreateForEntity("Dataset", datasetId);
            }
          
            return dataset;
        }     
    }
}
