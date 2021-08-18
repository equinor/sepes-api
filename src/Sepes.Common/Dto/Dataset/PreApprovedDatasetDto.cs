using Sepes.Common.Dto.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Common.Dto
{
    public class PreApprovedDatasetDto : DatasetCommon
    {
        // Not in spec
        public string SourceSystem { get; set; }
        public string BADataOwner { get; set; }
        public string Description { get; set; }
        public string Asset { get; set; }
        public string CountryOfOrigin { get; set; }
        public string AreaL1 { get; set; }
        public string AreaL2 { get; set; }
        public string Tags { get; set; }
    }
}
