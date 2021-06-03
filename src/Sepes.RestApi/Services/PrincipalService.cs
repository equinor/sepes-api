using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Principal;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Util.Auth;

namespace Sepes.RestApi.Services
{
    public class PrincipalService : IContextUserService
    {
        readonly IConfiguration _config;
        readonly IHttpContextAccessor _contextAccessor;
      

        public PrincipalService(IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _config = config;
            _contextAccessor = contextAccessor;
        }

        public IPrincipal GetPrincipal()
        {
            return _contextAccessor.HttpContext.User;
        }

        public bool IsEmployee()
        {
            return UserUtil.UserHasEmployeeRole(_config, GetPrincipal());
        }

        public bool IsAdmin()
        {
            return UserUtil.UserHasAdminRole(GetPrincipal());
        }

        public bool IsDatasetAdmin()
        {
            return UserUtil.UserHasDatasetAdminRole(GetPrincipal());
        }      

        public bool IsSponsor()
        {
            return UserUtil.UserHasSponsorRole(GetPrincipal());
        }
    }
}
