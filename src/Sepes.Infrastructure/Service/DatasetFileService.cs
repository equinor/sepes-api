using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Infrastructure.Service
{
    public class DatasetFileService : IDatasetFileService
    {
        readonly SepesDbContext _db;
        readonly ILogger _logger;
        readonly IUserService _userService;

        public DatasetFileService(SepesDbContext db, ILogger<DatasetFileService> logger, IUserService userService)
          
        {
            _db = db;
            _logger = logger;
            _userService = userService;
        }

       
        //public async Task<List<Guid>> AddFiles(int datasetId, List<IFormFile> files)
        //{

        //}
    }
}
