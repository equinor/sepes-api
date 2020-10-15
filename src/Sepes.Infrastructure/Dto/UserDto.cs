using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class UserDto
    {
        public int Id { get; set; }     

        public string ObjectId { get; set; }

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        public bool Admin { get; set; }

        public bool Sponsor { get; set; }

        public bool DatasetAdmin { get; set; }


        public List<StudyParticipantDto> StudyParticipants { get; set; }

        public UserDto()
        {
          
        }

        public UserDto(string objectId, string userName, string fullName, string email, bool admin = false, bool sponsor = false, bool datasetAdmin = false)
        {          
            ObjectId = objectId;
            UserName = userName;
            FullName = fullName;
            EmailAddress = email;

            Admin = admin;
            Sponsor = sponsor;
            DatasetAdmin = datasetAdmin;
        }  
    }
}
