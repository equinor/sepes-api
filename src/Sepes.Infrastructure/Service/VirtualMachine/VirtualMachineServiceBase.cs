using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Text.RegularExpressions;
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

        public void ValidateVmPasswordOrThrow(string password)
        {
            var errorString = "";
            //Atleast one upper case
            var upper = new Regex(@"(?=.*[A-Z])");
            //Atleast one number
            var number = new Regex(@".*[0-9].*");
            //Atleast one special character
            var special = new Regex(@"(?=.*[!@#$%^&*])");
            //Between 12-123 long
            var limit = new Regex(@"(?=.{12,123})");
            if (!upper.IsMatch(password))
            {
                errorString += "Missing one uppercase character. ";
            }
            if (!number.IsMatch(password))
            {
                errorString += "Missing one number. ";
            }
            if (!special.IsMatch(password))
            {
                errorString += "Missing one special character. ";
            }
            if (!limit.IsMatch(password))
            {
                errorString += "Outside the limit (12-123). ";

            }

            if (!String.IsNullOrWhiteSpace(errorString))
            {
                throw new Exception($"Password is missing following requirements: {errorString}");
            }
        }     

        protected async Task<CloudResource> GetVirtualMachineResourceEntry(int id, UserOperation operation, CancellationToken cancellation = default)
        {
            return await _cloudResourceReadService.GetByIdAsync(id, operation);
        } 
    }
}
