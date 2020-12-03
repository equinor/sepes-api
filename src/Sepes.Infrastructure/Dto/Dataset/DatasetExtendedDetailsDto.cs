namespace Sepes.Infrastructure.Dto.Dataset
{
    public class DatasetExtendedDetailsDto
    {
        public int DataId { get; set; }

        public string Description { get; set; }
      
        public int LRAId { get; set; }
     
        public string SourceSystem { get; set; }
        public string BADataOwner { get; set; }
        public string Asset { get; set; }
        public string CountryOfOrigin { get; set; }
        public string AreaL1 { get; set; }
        public string AreaL2 { get; set; }
        public string Tags { get; set; }
    }
}
