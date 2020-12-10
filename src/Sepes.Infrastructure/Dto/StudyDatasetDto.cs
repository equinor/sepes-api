using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class StudyDatasetDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Classification { get; set; }
        public string StorageAccountName { get; set; }

        public string StorageAccountLink { get; set; }

        public int LRAId { get; set; }
        public int DataId { get; set; }
        public string SourceSystem { get; set; }
        public string BADataOwner { get; set; }
        public string Asset { get; set; }
        public string CountryOfOrigin { get; set; }
        public string AreaL1 { get; set; }
        public string AreaL2 { get; set; }
        public string Tags { get; set; }       
    }
}
