using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public abstract class ModelServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;       

        public ModelServiceBase(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;           
            _logger = logger;            
        }
    }   

    public class EfModelServiceBase : ModelServiceBase
    {
        protected readonly SepesDbContext _db;      

        public EfModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger)
                   : base(configuration, logger)
        {
            _db = db;          
        }
    }     
}
