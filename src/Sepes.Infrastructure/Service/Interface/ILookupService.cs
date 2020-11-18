using Sepes.Infrastructure.Dto;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ILookupService
    {
        public IEnumerable<LookupDto> StudyRoles();

    }
}
