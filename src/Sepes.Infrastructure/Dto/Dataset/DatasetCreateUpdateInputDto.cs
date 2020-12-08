namespace Sepes.Infrastructure.Dto.Dataset
{
    public class DatasetCreateUpdateInputDto
    {
        public string Name { get; set; }    

        //public string StorageAccountName { get; set; }

        public string Location { get; set; }

        public string Classification { get; set; }

        public int DataId { get; set; }
    }
}
