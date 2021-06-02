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
        protected readonly IUserService _userService;

        public ModelServiceBase(IConfiguration configuration, ILogger logger, IUserService userService)
        {
            _configuration = configuration;           
            _logger = logger;
            _userService = userService;
        }
    }

   

    public class EfModelServiceBase : ModelServiceBase
    {
        protected readonly SepesDbContext _db;

        public EfModelServiceBase(IConfiguration configuration, SepesDbContext db, ILogger logger, IUserService userService)
                   : base(configuration, logger, userService)
        {
            _db = db;
        }
    }

     
}
