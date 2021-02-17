using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ILookupService
    {
        public IEnumerable<LookupDto> StudyRoles();
        public Task<IEnumerable<LookupDto>> StudyRolesUserCanGive(int studyId);

    }
}
