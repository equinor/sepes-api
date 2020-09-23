namespace Sepes.Infrastructure.Dto
{
    public class ParticipantLookupDto
    {
        public string FullName { get; set; }
        public string Source { get; set; }
        public int? DatabaseId { get; set; }
        public string ObjectId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }

    }
}
