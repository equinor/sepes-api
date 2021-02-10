using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Dataset
{
    public class DatasetListItemDto : LookupBaseDto
    {
        public int? StudyId { get; set; }

        public List<SandboxListItem> Sandboxes { get; set; }
    }
}
