using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Interface;
using System.Security.Claims;
using System.Security.Principal;

namespace Sepes.CloudResourceWorker.Service
{
    public class PrincipalService : IPrincipalService
    {
        //readonly IHttpContextAccessor _contextAccessor;

        //public PrincipalService(IHttpContextAccessor contextAccessor)
        //{
        //    _contextAccessor = contextAccessor;
        //}

        public PrincipalService()
        {
        
        }

        public IPrincipal GetPrincipal()
        { 
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, "025aa7b4-c274-4341-9e3f-f0a1af53a16a"),
                                        new Claim(ClaimTypes.Name, "test@equinor.com")
                                        // other required and custom claims
                                   }, "TestAuthentication"));         
        


            return user;
        /*    return _contextAccessor.HttpContext.User*/;
        }
    }
}
