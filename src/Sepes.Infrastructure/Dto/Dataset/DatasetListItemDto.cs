using Sepes.Infrastructure.Dto.Sandbox;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Dataset
{
    public class DatasetListItemDto : LookupBaseDto
    {
        public int? StudyId { get; set; }

        public List<SandboxListItemDto> Sandboxes { get; set; }
    }
}
