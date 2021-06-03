using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public abstract class ModelServiceBase
    {
        protected readonly IConfiguration _configuration;
       
        protected readonly ILogger _logger;
       

        public ModelServiceBase(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;           
            _logger = logger;
            
        }
    }

   

    public class EfModelServiceBase : ModelServiceBase
    {
        protected readonly SepesDbContext _db;
        protected readonly IUserService _userService;

        public EfModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger, IUserService userService)
                   : base(configuration, logger)
        {
            _db = db;
            _userService = userService;
        }
    }

     
}
