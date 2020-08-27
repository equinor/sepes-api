using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Service
{
    public class UserService : IUserService
    {
        readonly IHasPrincipal _principalService;
        readonly SepesUser _currentUser;         

        public UserService(IHasPrincipal principalService, SepesDbContext db)
     
        {
            this._principalService = principalService;
            _currentUser = CreateUser();
        }

        SepesUser CreateUser()
        {
            var user = UserUtil.CreateSepesUser(_principalService.GetPrincipal());
            return user;
        }

        public SepesUser GetCurrentUser()
        {
            return _currentUser;
        }
    }
}
