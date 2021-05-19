using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineServiceBase
    {
        protected readonly IConfiguration _config;
        protected readonly SepesDbContext _db;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;  
      
        readonly ICloudResourceReadService _cloudResourceReadService;    

        public VirtualMachineServiceBase(
             IConfiguration config,
            SepesDbContext db
            , ILogger logger,           
            IMapper mapper,
            IUserService userService, 
            ICloudResourceReadService cloudResourceReadService)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _mapper = mapper;
            _userService = userService;        
            _cloudResourceReadService = cloudResourceReadService;
        }          

        protected async Task<CloudResource> GetVirtualMachineResourceEntry(int id, UserOperation operation, CancellationToken cancellation = default)
        {
            return await _cloudResourceReadService.GetByIdAsync(id, operation);
        } 
    }
}
