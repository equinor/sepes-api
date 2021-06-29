using Sepes.Common.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IStudyParticipantRolesService
    {
        public Task<IEnumerable<LookupDto>> RolesAvailableForUser(int studyId);
    }
}
