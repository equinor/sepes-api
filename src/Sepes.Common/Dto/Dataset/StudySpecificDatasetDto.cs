using Sepes.Common.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Dto.Study;
using Sepes.Common.Dto.Dataset;

namespace Sepes.Common.Dto
{
    public class StudySpecificDatasetDto : DatasetCommon
    {
        public int Id { get; set; }
        public string StudyName { get; set; }
        public DatasetPermissionsDto Permissions { get; set; } = new DatasetPermissionsDto();
    }
}
