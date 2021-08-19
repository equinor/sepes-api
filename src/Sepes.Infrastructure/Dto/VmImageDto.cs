namespace Sepes.Infrastructure.Dto
{
    public class VmImageDto
    {
        public int Id { get; set; }

        public string ForeignSystemId { get; set; }

        
        public string Name { get; set; }

        
        public string DisplayValue { get; set; }

        public string DisplayValueExtended { get; set; }


        public string Category { get; set; }

        public bool Recommended { get; set; }       
    }
}
