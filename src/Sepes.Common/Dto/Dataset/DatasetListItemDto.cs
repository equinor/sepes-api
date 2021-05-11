using System.Collections.Generic;
using Sepes.Common.Response.Sandbox;

namespace Sepes.Common.Dto.Dataset
{
    public class DatasetListItemDto : LookupBaseDto
    {
        public int? StudyId { get; set; }

        public List<SandboxListItem> Sandboxes { get; set; }
    }
}
