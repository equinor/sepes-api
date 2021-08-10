namespace Sepes.Common.Dto
{
    public class DatasetCommon
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Classification { get; set; }
        public string StorageAccountName { get; set; }
        public int LRAId { get; set; }
        public int DataId { get; set; }

        public string StorageAccountLink { get; set; }
    }
}
