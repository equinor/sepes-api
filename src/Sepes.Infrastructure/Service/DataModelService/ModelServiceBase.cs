using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public abstract class ModelServiceBase
    {
        protected readonly ILogger _logger;            

        public ModelServiceBase(ILogger logger)
        {                   
            _logger = logger;            
        }
    }   

    public class EfModelServiceBase : ModelServiceBase
    {
        protected readonly SepesDbContext _db;
        protected readonly IConfiguration _configuration;

        public EfModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger)
                   : base(logger)
        {
            _configuration = configuration;
            _db = db;          
        }
    }     
}
