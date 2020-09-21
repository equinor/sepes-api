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

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }   


        public List<StudyParticipantDto> StudyParticipants { get; set; }

        public UserDto()
        {
          
        }

        public UserDto(string tenantId, string objectId, string userName, string fullName, string email)
        {         
            UserName = userName;
            FullName = fullName;
            Email = email;
            TenantId = tenantId;
            ObjectId = objectId;
        }

        public UserDto(ClaimsPrincipal principal, string tenantId, string objectId, string userName, string fullName, string email)
        {            
            Principal = principal;           
            UserName = userName;
            FullName = fullName;
            Email = email;
            TenantId = tenantId;
            ObjectId = objectId;
        }

        public UserDto(int id, string userName, string fullName, string email, string tenantId, string objectId)
        {
            Id = id;
            UserName = userName;
            FullName = fullName;
            Email = email;
            TenantId = tenantId;
            ObjectId = objectId;
        }

           
    }
}
