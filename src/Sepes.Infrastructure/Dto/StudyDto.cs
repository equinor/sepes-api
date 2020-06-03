namespace Sepes.Infrastructure.Dto
{
    public class StudyDto : UpdateableBaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string WbsCode { get; set; }
    }
}
