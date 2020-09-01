using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Security.Claims;

namespace Sepes.Infrastructure.Dto
{
    public class UserDto
    {
        public ClaimsPrincipal Principal { get; set; }

        public int Id { get; set; }

        public string TenantId { get; set; }

        public string ObjectId { get; set; }

        public string FullName { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }   


        public List<StudyParticipantDto> StudyParticipants { get; set; }

        public UserDto()
        {
          
        }

        public UserDto (ClaimsPrincipal principal)
        {
            Principal = principal;
            TenantId = UserUtil.GetTenantId(Principal);
            ObjectId = UserUtil.GetOid(Principal);  
            Email = UserUtil.GetEmail(Principal);
            FullName = UserUtil.GetFullName(Principal);
        }        
    }
}
