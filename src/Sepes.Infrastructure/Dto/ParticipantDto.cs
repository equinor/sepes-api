using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class ParticipantDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public string UserName { get; set; }

        public string EmailAddress { get; set; }

    }
}
