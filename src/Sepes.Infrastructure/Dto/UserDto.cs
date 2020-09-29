using System.Collections.Generic;
using System.Security.Claims;

namespace Sepes.Infrastructure.Dto
{
    public class UserDto
    {
        public ClaimsPrincipal Principal { get; set; }

        public int Id { get; set; }     

        public string ObjectId { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }   


        public List<StudyParticipantDto> StudyParticipants { get; set; }

        public UserDto()
        {
          
        }

        public UserDto(string objectId, string userName, string fullName, string email)
        {          
            ObjectId = objectId;
            UserName = userName;
            FullName = fullName;
            EmailAddress = email;  
        }

        public UserDto(ClaimsPrincipal principal, string objectId, string userName, string fullName, string email)
        {            
            Principal = principal;            
            ObjectId = objectId;
            UserName = userName;
            FullName = fullName;
            EmailAddress = email;          
          
        }          
    }
}
